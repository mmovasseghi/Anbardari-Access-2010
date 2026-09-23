using System;
using System.Data.OleDb;
using Anbarban.Data;

namespace Anbarban.Services
{
    public sealed class UnlockWorkflow
    {
        private readonly AccessConnectionFactory _db;
        private readonly DocumentPostService _post;

        public UnlockWorkflow(AccessConnectionFactory db)
        {
            _db = db;
            _post = new DocumentPostService(db);
        }

        public bool TryUnlockAndUnpost(string docKind, int documentId, string documentNumber, string enteredCode)
        {
            var code = (enteredCode ?? "").Trim().Replace(" ", "").ToUpperInvariant();
            if (code.Length != 6) return false;
            if (code != UnlockService.ComputeCode(docKind, documentId, documentNumber)) return false;

            using var conn = _db.Open();
            using var cmd = new OleDbCommand("SELECT ID FROM UsedUnlockCodes WHERE UnlockCode=?", conn);
            cmd.Parameters.Add(OleDbUtil.P("@c", code));
            using var r = cmd.ExecuteReader();
            if (r.Read()) return false;

            OleDbUtil.ExecuteNonQuery(conn, null,
                "INSERT INTO UsedUnlockCodes (UnlockCode, DocKind, DocumentID, UsedAt) VALUES (?,?,?,?)",
                OleDbUtil.P("@c", code), OleDbUtil.P("@k", docKind), OleDbUtil.P("@id", documentId),
                OleDbUtil.P("@t", DateTime.Now));

            if (docKind == "INCOMING")
            {
                var err = _post.UnpostIncoming(documentId);
                return err == null;
            }
            if (docKind == "OUTGOING")
            {
                var err = _post.UnpostOutgoing(documentId);
                return err == null;
            }
            return false;
        }
    }
}
