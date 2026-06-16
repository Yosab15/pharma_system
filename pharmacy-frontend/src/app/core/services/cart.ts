import { Injectable, signal, computed } from '@angular/core';

export interface CartItem {
  productId: number;
  name: string;
  price: number;
  imageUrl: string;
  quantity: number;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {

  items = signal<CartItem[]>([]);
  paymentType = signal<'Cash' | 'Deferred'>('Cash');
  freeUnitsReceived = signal<number>(0);

  totalItems = computed(() =>
    this.items().reduce((sum, i) => sum + i.quantity, 0)
  );

  totalPrice = computed(() =>
    this.items().reduce((sum, i) => sum + i.price * i.quantity, 0)
  );

  addItem(product: any) {
    const existing = this.items().find(i => i.productId === product.id);

    if (existing) {
      this.items.update(items =>
        items.map(i =>
          i.productId === product.id
            ? { ...i, quantity: i.quantity + 1 }
            : i
        )
      );
    } else {
      this.items.update(items => [
        ...items,
        {
          productId: product.id,
          name: product.name,
          price: product.price,
          imageUrl: product.imageUrl,
          quantity: 1
        }
      ]);
    }
  }

  removeItem(productId: number) {
    this.items.update(items =>
      items.filter(i => i.productId !== productId)
    );
  }

  updateQuantity(productId: number, quantity: number) {
    if (quantity <= 0) {
      this.removeItem(productId);
      return;
    }
    this.items.update(items =>
      items.map(i =>
        i.productId === productId ? { ...i, quantity } : i
      )
    );
  }

  clear() {
    this.items.set([]);
    this.paymentType.set('Cash');
    this.freeUnitsReceived.set(0);
  }
}
