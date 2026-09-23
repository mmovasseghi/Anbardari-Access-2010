#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""سیستم انبارداری پرتابل — اجرا با Python استاندارد (ویندوز/لینوکس)"""
from __future__ import annotations

import json
import tkinter as tk
from tkinter import ttk, messagebox
from pathlib import Path
from datetime import date

ROOT = Path(__file__).resolve().parent
DATA = ROOT.parent / "data" / "inventory.json"


def load_db():
    DATA.parent.mkdir(parents=True, exist_ok=True)
    if not DATA.exists():
        db = {"products": [], "documents": [], "items": [], "seq": {"product": 1, "document": 1, "item": 1}}
        save_db(db)
        return db
    return json.loads(DATA.read_text(encoding="utf-8"))


def save_db(db):
    DATA.parent.mkdir(parents=True, exist_ok=True)
    DATA.write_text(json.dumps(db, ensure_ascii=False, indent=2), encoding="utf-8")


class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("سیستم انبارداری")
        self.geometry("820x640")
        self.configure(bg="#ECF1F5")
        self.option_add("*Font", "Tahoma 11")
        self.db = load_db()
        self._current_doc = None
        self._mode = "IN"
        self.container = ttk.Frame(self)
        self.container.pack(fill="both", expand=True, padx=12, pady=12)
        self.show_home()

    def clear(self):
        for w in self.container.winfo_children():
            w.destroy()

    def header(self, title, subtitle=""):
        fr = tk.Frame(self.container, bg="#1C3A4D")
        fr.pack(fill="x", pady=(0, 10))
        tk.Label(fr, text=title, bg="#1C3A4D", fg="white", font=("Tahoma", 18, "bold")).pack(anchor="e", padx=12, pady=(10, 2))
        if subtitle:
            tk.Label(fr, text=subtitle, bg="#1C3A4D", fg="#D2DEE6", font=("Tahoma", 10)).pack(anchor="e", padx=12, pady=(0, 10))

    def back_btn(self):
        ttk.Button(self.container, text="بازگشت به منوی اصلی", command=self.show_home).pack(anchor="e", pady=8)

    def show_home(self):
        self.clear()
        self.header("سیستم انبارداری", "کار با دکمه‌ها — ساده و روزمره")
        btns = [
            ("ثبت ورود کالا", lambda: self.show_doc("IN")),
            ("ثبت خروج / حواله", lambda: self.show_doc("OUT")),
            ("مدیریت کالاها", self.show_products),
            ("جستجوی کالا", self.show_search),
            ("مشاهده موجودی انبار", lambda: self.show_stock(False)),
            ("گزارش ورود و خروج", self.show_inout),
            ("گزارش کالاهای کم‌موجودی", self.show_low),
            ("خروج از برنامه", self.destroy),
        ]
        for text, cmd in btns:
            b = tk.Button(self.container, text=text, command=cmd, font=("Tahoma", 13), bg="#146E6E", fg="white", height=2)
            if text.startswith("خروج"):
                b.configure(bg="#A03030")
            b.pack(fill="x", pady=4)

    def find_product(self, pid):
        for p in self.db["products"]:
            if p["ID"] == pid:
                return p
        return None

    def status(self, p):
        return "نیاز به تأمین" if p["CurrentStock"] <= p["MinimumStock"] else "موجود"

    def show_doc(self, mode):
        self.clear()
        self._mode = mode
        self._current_doc = None
        title = "ثبت خروج / حواله" if mode == "OUT" else "ثبت ورود کالا"
        self.header(title, "اول سند را ذخیره کنید، بعد اقلام را اضافه کنید")
        form = ttk.Frame(self.container)
        form.pack(fill="x")
        self.v_no = tk.StringVar()
        self.v_del = tk.StringVar()
        self.v_date = tk.StringVar(value=str(date.today()))
        self.v_src = tk.StringVar()
        self.v_dst = tk.StringVar()
        self.v_desc = tk.StringVar()
        rows = [("شماره سند", self.v_no)]
        if mode == "OUT":
            rows.append(("شماره حواله", self.v_del))
        rows += [("تاریخ", self.v_date), ("مبدأ", self.v_src), ("مقصد", self.v_dst), ("توضیحات", self.v_desc)]
        for i, (lab, var) in enumerate(rows):
            ttk.Label(form, text=lab).grid(row=i, column=1, sticky="e", pady=3, padx=6)
            ttk.Entry(form, textvariable=var, width=40, justify="right").grid(row=i, column=0, sticky="ew", pady=3)
        ttk.Button(self.container, text="ذخیره سند", command=self.save_doc).pack(anchor="e", pady=6)

        ttk.Separator(self.container).pack(fill="x", pady=8)
        ttk.Label(self.container, text="افزودن کالا").pack(anchor="e")
        self.v_pid = tk.StringVar()
        self.v_qty = tk.StringVar(value="1")
        names = [f"{p['ID']}|{p['ProductName']} ({p.get('ProductCode','')})" for p in self.db["products"] if p.get("IsActive", True)]
        self.cb = ttk.Combobox(self.container, values=names, width=50, justify="right")
        self.cb.pack(anchor="e", pady=3)
        ttk.Entry(self.container, textvariable=self.v_qty, width=10, justify="right").pack(anchor="e", pady=3)
        ttk.Button(self.container, text="افزودن کالا به سند", command=self.add_item).pack(anchor="e", pady=4)
        self.items_box = tk.Listbox(self.container, height=8, justify="right")
        self.items_box.pack(fill="both", expand=True, pady=6)
        self.back_btn()

    def save_doc(self):
        if self._mode == "OUT" and not self.v_del.get().strip():
            messagebox.showwarning("سیستم انبارداری", "برای خروج کالا، شماره حواله را وارد کنید.")
            return
        doc = {
            "ID": self.db["seq"]["document"],
            "DocumentNumber": self.v_no.get().strip(),
            "DeliveryNumber": self.v_del.get().strip(),
            "TransactionType": self._mode,
            "DocumentDate": self.v_date.get().strip() or str(date.today()),
            "Source": self.v_src.get().strip(),
            "Destination": self.v_dst.get().strip(),
            "Description": self.v_desc.get().strip(),
        }
        self.db["seq"]["document"] += 1
        self.db["documents"].append(doc)
        save_db(self.db)
        self._current_doc = doc["ID"]
        messagebox.showinfo("سیستم انبارداری", "سند ذخیره شد. حالا اقلام را اضافه کنید.")

    def add_item(self):
        if not self._current_doc:
            messagebox.showwarning("سیستم انبارداری", "ابتدا اطلاعات بالای سند را ذخیره کنید، بعد اقلام را وارد کنید.")
            return
        sel = self.cb.get()
        if "|" not in sel:
            messagebox.showwarning("سیستم انبارداری", "لطفاً کالا را از فهرست انتخاب کنید.")
            return
        pid = int(sel.split("|", 1)[0])
        try:
            qty = int(self.v_qty.get())
        except ValueError:
            qty = 0
        if qty <= 0:
            messagebox.showwarning("سیستم انبارداری", "تعداد باید بیشتر از صفر باشد.")
            return
        p = self.find_product(pid)
        if not p:
            return
        if self._mode == "OUT":
            if p["CurrentStock"] < qty:
                messagebox.showwarning("سیستم انبارداری", f"موجودی این کالا کافی نیست.\nموجودی فعلی: {p['CurrentStock']}")
                return
            p["CurrentStock"] -= qty
        else:
            p["CurrentStock"] += qty
        self.db["items"].append({"ID": self.db["seq"]["item"], "DocumentID": self._current_doc, "ProductID": pid, "Quantity": qty})
        self.db["seq"]["item"] += 1
        save_db(self.db)
        self.items_box.insert(0, f"{p['ProductName']} — {qty}")
        self.v_qty.set("1")

    def show_products(self):
        self.clear()
        self.header("مدیریت کالاها", "موجودی فقط با ورود و خروج عوض می‌شود")
        self.p_name = tk.StringVar()
        self.p_code = tk.StringVar()
        self.p_unit = tk.StringVar(value="عدد")
        self.p_min = tk.StringVar(value="0")
        for lab, var in [("نام کالا", self.p_name), ("کد کالا", self.p_code), ("واحد", self.p_unit), ("حداقل موجودی", self.p_min)]:
            ttk.Label(self.container, text=lab).pack(anchor="e")
            ttk.Entry(self.container, textvariable=var, justify="right").pack(fill="x", pady=2)
        ttk.Button(self.container, text="کالای جدید / ذخیره", command=self.save_product).pack(anchor="e", pady=6)
        cols = ("name", "code", "unit", "stock", "min", "st")
        tree = ttk.Treeview(self.container, columns=cols, show="headings", height=12)
        heads = ["نام", "کد", "واحد", "موجودی", "حداقل", "وضعیت"]
        for c, h in zip(cols, heads):
            tree.heading(c, text=h)
            tree.column(c, width=100, anchor="e")
        for p in self.db["products"]:
            tree.insert("", "end", values=(p["ProductName"], p.get("ProductCode", ""), p.get("Unit", ""), p["CurrentStock"], p["MinimumStock"], self.status(p)))
        tree.pack(fill="both", expand=True)
        self.back_btn()

    def save_product(self):
        name = self.p_name.get().strip()
        if not name:
            messagebox.showwarning("سیستم انبارداری", "نام کالا را وارد کنید.")
            return
        try:
            mn = int(self.p_min.get() or 0)
        except ValueError:
            mn = 0
        self.db["products"].append({
            "ID": self.db["seq"]["product"],
            "ProductName": name,
            "ProductCode": self.p_code.get().strip(),
            "Unit": self.p_unit.get().strip() or "عدد",
            "CurrentStock": 0,
            "MinimumStock": max(0, mn),
            "IsActive": True,
        })
        self.db["seq"]["product"] += 1
        save_db(self.db)
        self.show_products()

    def show_search(self):
        self.clear()
        self.header("جستجوی کالا", "نام کالا یا کد کالا را وارد کنید")
        self.q = tk.StringVar()
        ttk.Entry(self.container, textvariable=self.q, justify="right").pack(fill="x")
        ttk.Button(self.container, text="جستجو", command=self.do_search).pack(anchor="e", pady=4)
        self.search_list = tk.Listbox(self.container, justify="right", height=18)
        self.search_list.pack(fill="both", expand=True)
        self.do_search()
        self.back_btn()

    def do_search(self):
        self.search_list.delete(0, "end")
        q = self.q.get().strip().lower()
        for p in self.db["products"]:
            hay = f"{p['ProductName']} {p.get('ProductCode','')}".lower()
            if q and q not in hay:
                continue
            self.search_list.insert("end", f"{p['ProductName']} | {p.get('ProductCode','')} | موجودی {p['CurrentStock']} | {self.status(p)}")

    def show_stock(self, low_only=False):
        self.clear()
        self.header("مشاهده موجودی انبار")
        ttk.Button(self.container, text="نمایش فقط کالاهای کم‌موجودی", command=lambda: self.show_stock(True)).pack(anchor="e")
        ttk.Button(self.container, text="نمایش همه", command=lambda: self.show_stock(False)).pack(anchor="e")
        lb = tk.Listbox(self.container, justify="right", height=20)
        lb.pack(fill="both", expand=True, pady=6)
        for p in self.db["products"]:
            if not p.get("IsActive", True):
                continue
            if low_only and p["CurrentStock"] > p["MinimumStock"]:
                continue
            lb.insert("end", f"{p.get('ProductCode','')} | {p['ProductName']} | {p['CurrentStock']} | {self.status(p)}")
        self.back_btn()

    def show_low(self):
        self.show_stock(True)

    def show_inout(self):
        self.clear()
        self.header("گزارش ورود و خروج")
        lb = tk.Listbox(self.container, justify="right", height=22)
        lb.pack(fill="both", expand=True)
        for it in reversed(self.db["items"]):
            doc = next((d for d in self.db["documents"] if d["ID"] == it["DocumentID"]), None)
            p = self.find_product(it["ProductID"])
            if not doc:
                continue
            t = "ورود" if doc["TransactionType"] == "IN" else "خروج"
            lb.insert("end", f"{doc['DocumentDate']} | {t} | {p['ProductName'] if p else ''} | {it['Quantity']} | سند {doc.get('DocumentNumber','')} | حواله {doc.get('DeliveryNumber','')}")
        self.back_btn()


def main():
    App().mainloop()


if __name__ == "__main__":
    main()
