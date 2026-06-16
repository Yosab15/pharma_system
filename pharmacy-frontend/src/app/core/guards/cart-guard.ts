import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { CartService } from '../services/cart';

export const cartGuard: CanActivateFn = () => {
  const router = inject(Router);
  const cartService = inject(CartService);

  if (cartService.items().length === 0) {
    router.navigate(['/']);
    return false;
  }

  return true;
};
