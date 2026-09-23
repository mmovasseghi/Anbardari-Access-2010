var App = (function(){
  var db = { products:[], documents:[], items:[], seq:{product:1, document:1, item:1} };
  var dataPath = "";

  function hasActiveX(){
    try{ new ActiveXObject("Scripting.FileSystemObject"); return true; }catch(e){ return false; }
  }
  function fso(){ return new ActiveXObject("Scripting.FileSystemObject"); }
  function appDir(){
    var p = document.location.pathname.replace(/\//g,"\\");
    if(p.charAt(0)==="\\") p = p.substring(1);
    return p.substring(0, p.lastIndexOf("\\"));
  }
  function blankDb(){ return { products:[], documents:[], items:[], seq:{product:1, document:1, item:1} }; }
  function ensureData(){
    if(!hasActiveX()){
      try{
        var raw = window.localStorage.getItem("anbardari_db");
        db = raw ? eval("("+raw+")") : blankDb();
      }catch(e){ db = blankDb(); }
      normalizeDb();
      return;
    }
    var fs = fso();
    var dir = appDir();
    var dataDir = fs.GetParentFolderName(dir) + "\\data";
    if(!fs.FolderExists(dataDir)) fs.CreateFolder(dataDir);
    dataPath = dataDir + "\\inventory.json";
    if(!fs.FileExists(dataPath)) save();
    else load();
  }
  function normalizeDb(){
    if(!db || typeof db!=="object") db = blankDb();
    if(!db.seq) db.seq={product:1,document:1,item:1};
    if(!db.products) db.products=[];
    if(!db.documents) db.documents=[];
    if(!db.items) db.items=[];
  }
  function load(){
    try{
      var ts = fso().OpenTextFile(dataPath,1,false,-1);
      var txt = ts.ReadAll(); ts.Close();
      if(txt && txt.length>0) db = eval("("+txt+")");
      normalizeDb();
    }catch(e){ db = blankDb(); }
  }
  function save(){
    if(!hasActiveX()){
      try{ window.localStorage.setItem("anbardari_db", stringify(db)); }catch(e){}
      return;
    }
    var ts = fso().OpenTextFile(dataPath,2,true,-1);
    ts.Write(stringify(db));
    ts.Close();
  }
  function stringify(o){
    // minimal JSON for JScript
    if(o===null) return "null";
    var t=typeof o;
    if(t==="number"||t==="boolean") return String(o);
    if(t==="string") return "\""+o.replace(/\\/g,"\\\\").replace(/\"/g,"\\\"").replace(/\r/g,"\\r").replace(/\n/g,"\\n")+"\"";
    if(o instanceof Array){
      var a=[]; for(var i=0;i<o.length;i++) a.push(stringify(o[i])); return "["+a.join(",")+"]";
    }
    var parts=[], k; for(k in o){ if(o.hasOwnProperty(k)) parts.push(stringify(k)+":"+stringify(o[k])); }
    return "{"+parts.join(",")+"}";
  }
  function esc(s){ if(s===null||s===undefined) return ""; return String(s).replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;"); }
  function today(){
    var d=new Date();
    var m=d.getMonth()+1, day=d.getDate();
    return d.getFullYear()+"-"+(m<10?"0"+m:m)+"-"+(day<10?"0"+day:day);
  }
  function findProduct(id){
    for(var i=0;i<db.products.length;i++) if(db.products[i].ID==id) return db.products[i];
    return null;
  }
  function statusText(p){ return (p.CurrentStock<=p.MinimumStock)?"نیاز به تأمین":"موجود"; }
  function productOptions(selected){
    var html="<option value=''>-- انتخاب کالا --</option>";
    for(var i=0;i<db.products.length;i++){
      var p=db.products[i];
      if(!p.IsActive) continue;
      html+="<option value='"+p.ID+"'"+(String(selected)===String(p.ID)?" selected":"")+">"+esc(p.ProductName)+" ("+esc(p.ProductCode)+")</option>";
    }
    return html;
  }
  function backBtn(){ return "<div class='actions' style='margin-top:12px;'><button class='sec' onclick=\"show('home')\">بازگشت به منوی اصلی</button></div>"; }

  function init(){ ensureData(); }

  function renderDoc(tx){
    var id = (tx==="OUT")?"view-out":"view-in";
    var title = (tx==="OUT")?"ثبت خروج / حواله":"ثبت ورود کالا";
    var html="";
    html+="<div class='brand'><h1>"+title+"</h1><p>اطلاعات سند را پر کنید، ذخیره کنید، بعد اقلام را اضافه کنید.</p></div>";
    html+="<div class='panel'>";
    html+="<div class='row'><label>شماره سند</label><input id='d_no'/></div>";
    if(tx==="OUT") html+="<div class='row'><label>شماره حواله</label><input id='d_delivery'/></div>";
    html+="<div class='row'><label>تاریخ</label><input id='d_date' value='"+today()+"'/></div>";
    html+="<div class='row'><label>مبدأ</label><input id='d_source'/></div>";
    html+="<div class='row'><label>مقصد</label><input id='d_dest'/></div>";
    html+="<div class='row'><label>توضیحات</label><input id='d_desc'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick=\"App.saveDoc('"+tx+"')\">ذخیره سند</button></div>";
    html+="<p class='muted' id='d_msg'></p>";
    html+="<h3>اقلام سند</h3>";
    html+="<div class='row'><label>کالا</label><select id='i_product'>"+productOptions("")+"</select></div>";
    html+="<div class='row'><label>تعداد</label><input id='i_qty' value='1'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick=\"App.addItem('"+tx+"')\">افزودن کالا به سند</button></div>";
    html+="<div id='d_items'></div>";
    html+=backBtn();
    html+="</div>";
    document.getElementById(id).innerHTML=html;
    window._currentDocId=null;
  }

  function saveDoc(tx){
    var no=document.getElementById("d_no").value;
    var delivery= tx==="OUT" ? document.getElementById("d_delivery").value : "";
    var dt=document.getElementById("d_date").value;
    var source=document.getElementById("d_source").value;
    var dest=document.getElementById("d_dest").value;
    var desc=document.getElementById("d_desc").value;
    if(tx==="OUT" && (!delivery || delivery.replace(/\s/g,"")==="")){
      alert("برای خروج کالا، شماره حواله را وارد کنید."); return;
    }
    var doc={
      ID: db.seq.document++,
      DocumentNumber: no,
      DeliveryNumber: delivery,
      TransactionType: tx,
      DocumentDate: dt||today(),
      Source: source,
      Destination: dest,
      Description: desc
    };
    db.documents.push(doc);
    save();
    window._currentDocId=doc.ID;
    document.getElementById("d_msg").innerHTML="<span class='ok'>سند ذخیره شد. حالا اقلام را اضافه کنید.</span>";
    renderItems(doc.ID);
  }

  function addItem(tx){
    if(!window._currentDocId){ alert("ابتدا اطلاعات بالای سند را ذخیره کنید، بعد اقلام را وارد کنید."); return; }
    var pid=parseInt(document.getElementById("i_product").value,10);
    var qty=parseInt(document.getElementById("i_qty").value,10);
    if(!pid){ alert("لطفاً کالا را از فهرست انتخاب کنید."); return; }
    if(!qty || qty<=0){ alert("تعداد باید بیشتر از صفر باشد."); return; }
    var p=findProduct(pid);
    if(!p){ alert("کالای انتخاب‌شده پیدا نشد."); return; }
    if(tx==="OUT"){
      if(p.CurrentStock < qty){
        alert("موجودی این کالا کافی نیست.\nموجودی فعلی: "+p.CurrentStock);
        return;
      }
      p.CurrentStock -= qty;
    }else{
      p.CurrentStock += qty;
    }
    db.items.push({ID:db.seq.item++, DocumentID:window._currentDocId, ProductID:pid, Quantity:qty});
    save();
    document.getElementById("i_qty").value="1";
    renderItems(window._currentDocId);
  }

  function renderItems(docId){
    var html="<table><tr><th>کالا</th><th>تعداد</th><th></th></tr>";
    for(var i=0;i<db.items.length;i++){
      var it=db.items[i];
      if(it.DocumentID!=docId) continue;
      var p=findProduct(it.ProductID);
      html+="<tr><td>"+esc(p?p.ProductName:"")+"</td><td>"+it.Quantity+"</td>";
      html+="<td><button class='sec' style='width:auto;padding:4px 8px;' onclick='App.deleteItem("+it.ID+")'>حذف</button></td></tr>";
    }
    html+="</table>";
    var el=document.getElementById("d_items");
    if(el) el.innerHTML=html;
  }

  function deleteItem(itemId){
    var it=null, idx=-1;
    for(var i=0;i<db.items.length;i++) if(db.items[i].ID==itemId){ it=db.items[i]; idx=i; break; }
    if(!it) return;
    var doc=null;
    for(var j=0;j<db.documents.length;j++) if(db.documents[j].ID==it.DocumentID){ doc=db.documents[j]; break; }
    var p=findProduct(it.ProductID);
    if(doc && p){
      if(doc.TransactionType==="IN") p.CurrentStock -= it.Quantity;
      else p.CurrentStock += it.Quantity;
      if(p.CurrentStock<0) p.CurrentStock=0;
    }
    db.items.splice(idx,1);
    save();
    if(window._currentDocId) renderItems(window._currentDocId);
  }

  function renderProducts(){
    var html="<div class='brand'><h1>مدیریت کالاها</h1><p>موجودی فقط با ورود و خروج عوض می‌شود.</p></div><div class='panel'>";
    html+="<div class='row'><label>نام کالا</label><input id='p_name'/></div>";
    html+="<div class='row'><label>کد کالا</label><input id='p_code'/></div>";
    html+="<div class='row'><label>واحد</label><input id='p_unit' value='عدد'/></div>";
    html+="<div class='row'><label>حداقل موجودی</label><input id='p_min' value='0'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick='App.saveProduct()'>کالای جدید / ذخیره</button></div>";
    html+="<h3>فهرست کالاها</h3><table><tr><th>نام</th><th>کد</th><th>واحد</th><th>موجودی</th><th>حداقل</th><th>وضعیت</th><th>فعال</th></tr>";
    for(var i=0;i<db.products.length;i++){
      var p=db.products[i];
      html+="<tr><td>"+esc(p.ProductName)+"</td><td>"+esc(p.ProductCode)+"</td><td>"+esc(p.Unit)+"</td><td>"+p.CurrentStock+"</td><td>"+p.MinimumStock+"</td><td>"+statusText(p)+"</td><td>"+(p.IsActive?"بله":"خیر")+"</td></tr>";
    }
    html+="</table>"+backBtn()+"</div>";
    document.getElementById("view-products").innerHTML=html;
  }

  function saveProduct(){
    var name=document.getElementById("p_name").value;
    if(!name || name.replace(/\s/g,"")===""){ alert("نام کالا را وارد کنید."); return; }
    var code=document.getElementById("p_code").value;
    var unit=document.getElementById("p_unit").value||"عدد";
    var minq=parseInt(document.getElementById("p_min").value,10); if(isNaN(minq)||minq<0) minq=0;
    db.products.push({ID:db.seq.product++, ProductName:name, ProductCode:code, Unit:unit, CurrentStock:0, MinimumStock:minq, IsActive:true});
    save();
    renderProducts();
  }

  function renderSearch(){
    var html="<div class='brand'><h1>جستجوی کالا</h1><p>نام کالا یا کد کالا را وارد کنید</p></div><div class='panel'>";
    html+="<div class='row'><input id='s_q' placeholder='نام یا کد کالا'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick='App.doSearch()'>جستجو</button> <button class='sec' style='width:auto;' onclick=\"document.getElementById('s_q').value='';App.doSearch();\">پاک کردن فیلتر</button></div>";
    html+="<div id='s_res'></div>"+backBtn()+"</div>";
    document.getElementById("view-search").innerHTML=html;
    doSearch();
  }
  function doSearch(){
    var q=(document.getElementById("s_q").value||"").toLowerCase();
    var html="<table><tr><th>نام</th><th>کد</th><th>واحد</th><th>موجودی</th><th>حداقل</th><th>وضعیت</th><th></th></tr>";
    for(var i=0;i<db.products.length;i++){
      var p=db.products[i];
      var hay=(p.ProductName+" "+p.ProductCode).toLowerCase();
      if(q && hay.indexOf(q)<0) continue;
      html+="<tr><td>"+esc(p.ProductName)+"</td><td>"+esc(p.ProductCode)+"</td><td>"+esc(p.Unit)+"</td><td>"+p.CurrentStock+"</td><td>"+p.MinimumStock+"</td><td>"+statusText(p)+"</td>";
      html+="<td><button class='sec' style='width:auto;padding:4px 8px;' onclick='App.openMove("+p.ID+")'>گردش کالا</button></td></tr>";
    }
    html+="</table>";
    document.getElementById("s_res").innerHTML=html;
  }

  function renderStock(lowOnly){
    var html="<div class='brand'><h1>مشاهده موجودی انبار</h1><p>لیست موجودی برای کار روزمره</p></div><div class='panel'>";
    html+="<div class='row'><input id='st_q' placeholder='جستجو بر اساس نام یا کد'/></div>";
    html+="<div class='actions'>";
    html+="<button class='main' style='width:auto;' onclick='App.refreshStock(false)'>جستجو / نمایش همه</button> ";
    html+="<button class='sec' style='width:auto;' onclick='App.refreshStock(true)'>نمایش فقط کالاهای کم‌موجودی</button>";
    html+="</div><div id='st_res'></div>"+backBtn()+"</div>";
    document.getElementById("view-stock").innerHTML=html;
    refreshStock(!!lowOnly);
  }
  function refreshStock(lowOnly){
    var q="";
    try{ q=(document.getElementById("st_q").value||"").toLowerCase(); }catch(e){}
    var html="<table><tr><th>کد</th><th>نام</th><th>واحد</th><th>موجودی فعلی</th><th>حداقل</th><th>وضعیت</th><th></th></tr>";
    for(var i=0;i<db.products.length;i++){
      var p=db.products[i];
      if(!p.IsActive) continue;
      if(lowOnly && p.CurrentStock>p.MinimumStock) continue;
      var hay=(p.ProductName+" "+p.ProductCode).toLowerCase();
      if(q && hay.indexOf(q)<0) continue;
      html+="<tr><td>"+esc(p.ProductCode)+"</td><td>"+esc(p.ProductName)+"</td><td>"+esc(p.Unit)+"</td><td>"+p.CurrentStock+"</td><td>"+p.MinimumStock+"</td><td>"+statusText(p)+"</td>";
      html+="<td><button class='sec' style='width:auto;padding:4px 8px;' onclick='App.openMove("+p.ID+")'>گردش کالا</button></td></tr>";
    }
    html+="</table>";
    document.getElementById("st_res").innerHTML=html;
  }

  function renderLow(){
    var html="<div class='brand'><h1>گزارش کالاهای کم‌موجودی</h1><p>کالاهایی که رو به اتمام هستند</p></div><div class='panel'>";
    html+="<table><tr><th>کد</th><th>نام</th><th>واحد</th><th>موجودی فعلی</th><th>حداقل موجودی</th></tr>";
    for(var i=0;i<db.products.length;i++){
      var p=db.products[i];
      if(!p.IsActive) continue;
      if(p.CurrentStock>p.MinimumStock) continue;
      html+="<tr><td>"+esc(p.ProductCode)+"</td><td>"+esc(p.ProductName)+"</td><td>"+esc(p.Unit)+"</td><td>"+p.CurrentStock+"</td><td>"+p.MinimumStock+"</td></tr>";
    }
    html+="</table>"+backBtn()+"</div>";
    document.getElementById("view-low").innerHTML=html;
  }

  function renderInOut(){
    var html="<div class='brand'><h1>گزارش ورود و خروج</h1><p>فیلتر گزارش</p></div><div class='panel'>";
    html+="<div class='row'><label>نوع گزارش</label><select id='f_type'><option value=''>همه</option><option value='IN'>ورود کالا</option><option value='OUT'>خروج / حواله</option></select></div>";
    html+="<div class='row'><label>از تاریخ</label><input id='f_from'/></div>";
    html+="<div class='row'><label>تا تاریخ</label><input id='f_to'/></div>";
    html+="<div class='row'><label>کالا</label><select id='f_product'><option value=''>همه کالاها</option>"+productOptions("")+"</select></div>";
    html+="<div class='row'><label>شماره سند</label><input id='f_doc'/></div>";
    html+="<div class='row'><label>شماره حواله</label><input id='f_del'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick='App.runInOut()'>نمایش گزارش</button> <button class='sec' style='width:auto;' onclick='App.renderInOut()'>پاک کردن فیلتر</button></div>";
    html+="<div id='f_res'></div>"+backBtn()+"</div>";
    document.getElementById("view-inout").innerHTML=html;
    runInOut();
  }
  function runInOut(){
    var type=document.getElementById("f_type").value;
    var from=document.getElementById("f_from").value;
    var to=document.getElementById("f_to").value;
    var pid=document.getElementById("f_product").value;
    var docno=document.getElementById("f_doc").value;
    var delno=document.getElementById("f_del").value;
    var html="<table><tr><th>تاریخ</th><th>شماره سند</th><th>شماره حواله</th><th>نوع عملیات</th><th>کالا</th><th>کد</th><th>تعداد</th><th>مبدأ</th><th>مقصد</th><th>توضیحات</th></tr>";
    for(var i=0;i<db.items.length;i++){
      var it=db.items[i];
      var doc=null; for(var j=0;j<db.documents.length;j++) if(db.documents[j].ID==it.DocumentID){doc=db.documents[j];break;}
      if(!doc) continue;
      var p=findProduct(it.ProductID);
      if(type && doc.TransactionType!==type) continue;
      if(from && doc.DocumentDate<from) continue;
      if(to && doc.DocumentDate>to) continue;
      if(pid && String(it.ProductID)!==String(pid)) continue;
      if(docno && String(doc.DocumentNumber).indexOf(docno)<0) continue;
      if(delno && String(doc.DeliveryNumber||"").indexOf(delno)<0) continue;
      html+="<tr><td>"+esc(doc.DocumentDate)+"</td><td>"+esc(doc.DocumentNumber)+"</td><td>"+esc(doc.DeliveryNumber)+"</td><td>"+(doc.TransactionType==="IN"?"ورود":"خروج")+"</td><td>"+esc(p?p.ProductName:"")+"</td><td>"+esc(p?p.ProductCode:"")+"</td><td>"+it.Quantity+"</td><td>"+esc(doc.Source)+"</td><td>"+esc(doc.Destination)+"</td><td>"+esc(doc.Description)+"</td></tr>";
    }
    html+="</table>";
    document.getElementById("f_res").innerHTML=html;
  }

  function openMove(pid){
    show("move");
    var p=findProduct(pid);
    var html="<div class='brand'><h1>گزارش گردش کالا</h1><p>"+esc(p?p.ProductName:"")+"</p></div><div class='panel'>";
    html+="<p>کد: <b>"+esc(p?p.ProductCode:"")+"</b> | موجودی فعلی: <b>"+(p?p.CurrentStock:0)+"</b> | وضعیت: <b>"+(p?statusText(p):"")+"</b></p>";
    html+="<div class='row'><label>از تاریخ</label><input id='m_from'/></div>";
    html+="<div class='row'><label>تا تاریخ</label><input id='m_to'/></div>";
    html+="<div class='actions'><button class='main' style='width:auto;' onclick='App.runMove("+pid+")'>نمایش گزارش</button></div>";
    html+="<div id='m_res'></div>"+backBtn()+"</div>";
    document.getElementById("view-move").innerHTML=html;
    runMove(pid);
  }
  function runMove(pid){
    var from="", to="";
    try{ from=document.getElementById("m_from").value; to=document.getElementById("m_to").value; }catch(e){}
    var html="<table><tr><th>تاریخ</th><th>شماره سند</th><th>شماره حواله</th><th>نوع</th><th>تعداد</th><th>مبدأ</th><th>مقصد</th></tr>";
    for(var i=0;i<db.items.length;i++){
      var it=db.items[i];
      if(it.ProductID!=pid) continue;
      var doc=null; for(var j=0;j<db.documents.length;j++) if(db.documents[j].ID==it.DocumentID){doc=db.documents[j];break;}
      if(!doc) continue;
      if(from && doc.DocumentDate<from) continue;
      if(to && doc.DocumentDate>to) continue;
      html+="<tr><td>"+esc(doc.DocumentDate)+"</td><td>"+esc(doc.DocumentNumber)+"</td><td>"+esc(doc.DeliveryNumber)+"</td><td>"+(doc.TransactionType==="IN"?"ورود":"خروج")+"</td><td>"+it.Quantity+"</td><td>"+esc(doc.Source)+"</td><td>"+esc(doc.Destination)+"</td></tr>";
    }
    html+="</table>";
    document.getElementById("m_res").innerHTML=html;
  }

  return {
    init:init,
    renderDoc:renderDoc,
    saveDoc:saveDoc,
    addItem:addItem,
    deleteItem:deleteItem,
    renderProducts:renderProducts,
    saveProduct:saveProduct,
    renderSearch:renderSearch,
    doSearch:doSearch,
    renderStock:renderStock,
    refreshStock:refreshStock,
    renderLow:renderLow,
    renderInOut:renderInOut,
    runInOut:runInOut,
    openMove:openMove,
    runMove:runMove
  };
})();
