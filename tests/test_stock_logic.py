#!/usr/bin/env python3
"""
Access 2010 inventory logic simulator / test suite.
Mirrors VBA rules in modStock / modValidation / form events.
Run: python3 tests/test_stock_logic.py
"""
from __future__ import annotations

from dataclasses import dataclass, field
from typing import Dict, List, Optional
import unittest


TRANSACTION_IN = "IN"
TRANSACTION_OUT = "OUT"


@dataclass
class Product:
    id: int
    name: str
    code: str
    unit: str
    current_stock: int
    minimum_stock: int
    is_active: bool = True


@dataclass
class Document:
    id: int
    document_number: str
    delivery_number: Optional[str]
    transaction_type: str
    items: List["DocumentItem"] = field(default_factory=list)


@dataclass
class DocumentItem:
    id: int
    document_id: int
    product_id: int
    quantity: int


class InventoryApp:
    def __init__(self) -> None:
        self.products: Dict[int, Product] = {}
        self.documents: Dict[int, Document] = {}
        self._next_product = 1
        self._next_doc = 1
        self._next_item = 1
        self.errors: List[str] = []

    def clear_errors(self) -> None:
        self.errors.clear()

    def add_product(self, name: str, code: str, unit: str = "عدد", stock: int = 0, minimum: int = 0) -> Product:
        p = Product(self._next_product, name, code, unit, stock, minimum, True)
        self.products[p.id] = p
        self._next_product += 1
        return p

    def create_document(self, number: str, tx: str, delivery: Optional[str] = None) -> Document:
        if tx not in (TRANSACTION_IN, TRANSACTION_OUT):
            raise ValueError("invalid tx")
        if tx == TRANSACTION_OUT and (delivery is None or str(delivery).strip() == ""):
            raise ValueError("delivery required")
        d = Document(self._next_doc, number, delivery, tx)
        self.documents[d.id] = d
        self._next_doc += 1
        return d

    def validate_header(self, tx: Optional[str], delivery: Optional[str]) -> bool:
        self.clear_errors()
        if not tx:
            self.errors.append("tx required")
            return False
        if tx not in (TRANSACTION_IN, TRANSACTION_OUT):
            self.errors.append("tx invalid")
            return False
        if tx == TRANSACTION_OUT and (delivery is None or str(delivery).strip() == ""):
            self.errors.append("delivery required")
            return False
        return True

    def apply_stock_change(self, product_id: int, quantity: int, tx: str, delta_sign: int) -> bool:
        self.clear_errors()
        if product_id <= 0:
            self.errors.append("product required")
            return False
        if quantity <= 0:
            self.errors.append("qty must be > 0")
            return False
        if product_id not in self.products:
            self.errors.append("product not found")
            return False
        tx = tx.upper().strip()
        if tx == TRANSACTION_IN:
            signed = quantity * delta_sign
        elif tx == TRANSACTION_OUT:
            signed = -quantity * delta_sign
        else:
            self.errors.append("tx invalid")
            return False
        new_stock = self.products[product_id].current_stock + signed
        if new_stock < 0:
            self.errors.append("stock negative")
            return False
        self.products[product_id].current_stock = new_stock
        return True

    def add_item(self, doc_id: int, product_id: int, quantity: int) -> Optional[DocumentItem]:
        doc = self.documents[doc_id]
        if not self.validate_header(doc.transaction_type, doc.delivery_number):
            return None
        if quantity <= 0:
            self.errors.append("qty must be > 0")
            return None
        if not self.apply_stock_change(product_id, quantity, doc.transaction_type, 1):
            return None
        item = DocumentItem(self._next_item, doc_id, product_id, quantity)
        self._next_item += 1
        doc.items.append(item)
        return item

    def edit_item(self, doc_id: int, item_id: int, product_id: int, quantity: int) -> bool:
        doc = self.documents[doc_id]
        item = next(i for i in doc.items if i.id == item_id)
        # reverse old
        if not self.apply_stock_change(item.product_id, item.quantity, doc.transaction_type, -1):
            return False
        if not self.apply_stock_change(product_id, quantity, doc.transaction_type, 1):
            # restore
            self.apply_stock_change(item.product_id, item.quantity, doc.transaction_type, 1)
            return False
        item.product_id = product_id
        item.quantity = quantity
        return True

    def delete_item_confirmed(self, doc_id: int, item_id: int) -> bool:
        """Mirrors AfterDelConfirm with Status=acDeleteOK (fixed behavior)."""
        doc = self.documents[doc_id]
        item = next(i for i in doc.items if i.id == item_id)
        if not self.apply_stock_change(item.product_id, item.quantity, doc.transaction_type, -1):
            return False
        doc.items = [i for i in doc.items if i.id != item_id]
        return True

    def has_items(self, doc_id: int) -> bool:
        return len(self.documents[doc_id].items) > 0


class TestInventoryLogic(unittest.TestCase):
    def setUp(self) -> None:
        self.app = InventoryApp()
        self.p1 = self.app.add_product("کاغذ A4", "P-001", "بسته", 0, 10)
        self.p2 = self.app.add_product("جوهر", "P-002", "عدد", 0, 5)

    def test_in_increases_stock(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        self.assertIsNotNone(self.app.add_item(d.id, self.p1.id, 10))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 10)

    def test_out_decreases_stock(self) -> None:
        d_in = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d_in.id, self.p1.id, 10)
        d_out = self.app.create_document("OUT-1", TRANSACTION_OUT, "DLV-1")
        self.assertIsNotNone(self.app.add_item(d_out.id, self.p1.id, 3))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 7)

    def test_out_cannot_go_negative(self) -> None:
        d_in = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d_in.id, self.p1.id, 5)
        d_out = self.app.create_document("OUT-1", TRANSACTION_OUT, "DLV-1")
        self.assertIsNone(self.app.add_item(d_out.id, self.p1.id, 6))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 5)
        self.assertIn("stock negative", self.app.errors)

    def test_quantity_must_be_positive(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        self.assertIsNone(self.app.add_item(d.id, self.p1.id, 0))
        self.assertIsNone(self.app.add_item(d.id, self.p1.id, -2))

    def test_out_requires_delivery_number(self) -> None:
        with self.assertRaises(ValueError):
            self.app.create_document("OUT-1", TRANSACTION_OUT, "")
        self.assertFalse(self.app.validate_header(TRANSACTION_OUT, None))
        self.assertFalse(self.app.validate_header(TRANSACTION_OUT, "  "))

    def test_multi_item_document(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d.id, self.p1.id, 4)
        self.app.add_item(d.id, self.p2.id, 8)
        self.assertEqual(len(d.items), 2)
        self.assertEqual(self.app.products[self.p1.id].current_stock, 4)
        self.assertEqual(self.app.products[self.p2.id].current_stock, 8)

    def test_edit_item_adjusts_stock(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        item = self.app.add_item(d.id, self.p1.id, 5)
        assert item is not None
        self.assertTrue(self.app.edit_item(d.id, item.id, self.p1.id, 9))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 9)

    def test_edit_out_blocked_when_insufficient(self) -> None:
        d_in = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d_in.id, self.p1.id, 5)
        d_out = self.app.create_document("OUT-1", TRANSACTION_OUT, "DLV-1")
        item = self.app.add_item(d_out.id, self.p1.id, 2)
        assert item is not None
        self.assertFalse(self.app.edit_item(d_out.id, item.id, self.p1.id, 6))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 3)

    def test_delete_item_restores_stock(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        item = self.app.add_item(d.id, self.p1.id, 7)
        assert item is not None
        self.assertTrue(self.app.delete_item_confirmed(d.id, item.id))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 0)
        self.assertEqual(len(d.items), 0)

    def test_delete_out_item_returns_stock(self) -> None:
        d_in = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d_in.id, self.p1.id, 10)
        d_out = self.app.create_document("OUT-1", TRANSACTION_OUT, "DLV-9")
        item = self.app.add_item(d_out.id, self.p1.id, 4)
        assert item is not None
        self.assertTrue(self.app.delete_item_confirmed(d_out.id, item.id))
        self.assertEqual(self.app.products[self.p1.id].current_stock, 10)

    def test_low_stock_detection(self) -> None:
        d = self.app.create_document("IN-1", TRANSACTION_IN)
        self.app.add_item(d.id, self.p1.id, 3)  # min 10
        low = [p for p in self.app.products.values() if p.is_active and p.current_stock <= p.minimum_stock]
        self.assertEqual({p.code for p in low}, {"P-001", "P-002"})

    def test_combo_display_maps_to_stored_values(self) -> None:
        display = {"ورود": "IN", "خروج": "OUT"}
        self.assertEqual(display["ورود"], TRANSACTION_IN)
        self.assertEqual(display["خروج"], TRANSACTION_OUT)


if __name__ == "__main__":
    unittest.main(verbosity=2)
