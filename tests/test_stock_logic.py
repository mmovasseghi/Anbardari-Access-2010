#!/usr/bin/env python3
"""
Anbarban logic tests — two-phase posting, suppliers, departments.
Run: python3 tests/test_stock_logic.py
"""
from __future__ import annotations

from dataclasses import dataclass, field
from typing import Dict, List, Optional
import unittest


@dataclass
class Product:
    id: int
    name: str
    current_stock: int
    minimum_stock: int = 0


@dataclass
class IncomingDoc:
    id: int
    items: List[tuple] = field(default_factory=list)  # (product_id, qty)
    is_posted: bool = False


@dataclass
class OutgoingDoc:
    id: int
    items: List[tuple] = field(default_factory=list)  # (product_id, qty, dept_id)
    is_posted: bool = False


class AnbarbanApp:
    def __init__(self) -> None:
        self.products: Dict[int, Product] = {}
        self.incoming: Dict[int, IncomingDoc] = {}
        self.outgoing: Dict[int, OutgoingDoc] = {}
        self._next_p = 1
        self._next_in = 1
        self._next_out = 1

    def add_product(self, name: str, stock: int = 0, minimum: int = 0) -> Product:
        p = Product(self._next_p, name, stock, minimum)
        self.products[p.id] = p
        self._next_p += 1
        return p

    def draft_incoming(self) -> IncomingDoc:
        d = IncomingDoc(self._next_in)
        self.incoming[d.id] = d
        self._next_in += 1
        return d

    def add_incoming_item(self, doc_id: int, product_id: int, qty: int) -> None:
        doc = self.incoming[doc_id]
        assert not doc.is_posted
        assert qty > 0
        doc.items.append((product_id, qty))

    def post_incoming(self, doc_id: int) -> bool:
        doc = self.incoming[doc_id]
        if doc.is_posted or not doc.items:
            return False
        for pid, qty in doc.items:
            self.products[pid].current_stock += qty
        doc.is_posted = True
        return True

    def draft_outgoing(self) -> OutgoingDoc:
        d = OutgoingDoc(self._next_out)
        self.outgoing[d.id] = d
        self._next_out += 1
        return d

    def add_outgoing_item(self, doc_id: int, product_id: int, qty: int, dept: int = 1) -> bool:
        doc = self.outgoing[doc_id]
        if doc.is_posted or qty <= 0:
            return False
        need = qty + sum(q for p, q, _ in doc.items if p == product_id)
        if need > self.products[product_id].current_stock:
            return False
        doc.items.append((product_id, qty, dept))
        return True

    def post_outgoing(self, doc_id: int) -> bool:
        doc = self.outgoing[doc_id]
        if doc.is_posted or not doc.items:
            return False
        totals: Dict[int, int] = {}
        for pid, qty, _ in doc.items:
            totals[pid] = totals.get(pid, 0) + qty
        for pid, need in totals.items():
            if self.products[pid].current_stock < need:
                return False
        for pid, need in totals.items():
            self.products[pid].current_stock -= need
        doc.is_posted = True
        return True


class TestAnbarban(unittest.TestCase):
    def setUp(self) -> None:
        self.app = AnbarbanApp()
        self.p = self.app.add_product("کاغذ A4", 0, 10)

    def test_draft_incoming_no_stock_change(self) -> None:
        d = self.app.draft_incoming()
        self.app.add_incoming_item(d.id, self.p.id, 20)
        self.assertEqual(self.p.current_stock, 0)
        self.assertTrue(self.app.post_incoming(d.id))
        self.assertEqual(self.p.current_stock, 20)

    def test_outgoing_blocked_over_stock(self) -> None:
        d_in = self.app.draft_incoming()
        self.app.add_incoming_item(d_in.id, self.p.id, 10)
        self.app.post_incoming(d_in.id)
        d_out = self.app.draft_outgoing()
        self.assertFalse(self.app.add_outgoing_item(d_out.id, self.p.id, 15))
        self.assertEqual(self.p.current_stock, 10)

    def test_post_outgoing_reduces(self) -> None:
        d_in = self.app.draft_incoming()
        self.app.add_incoming_item(d_in.id, self.p.id, 10)
        self.app.post_incoming(d_in.id)
        d_out = self.app.draft_outgoing()
        self.app.add_outgoing_item(d_out.id, self.p.id, 4)
        self.assertTrue(self.app.post_outgoing(d_out.id))
        self.assertEqual(self.p.current_stock, 6)


if __name__ == "__main__":
    unittest.main(verbosity=2)
