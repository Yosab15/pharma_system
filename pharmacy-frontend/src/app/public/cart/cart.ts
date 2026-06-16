import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cart';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart {

  constructor(
    public cartService: CartService,
    private router: Router
  ) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goToOrder() {
    this.router.navigate(['/order']);
  }

}
