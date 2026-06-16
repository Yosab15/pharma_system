import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  if (!token) {
    router.navigate(['/admin/login']);
    return false;
  }

  // شيك على الـ expiry بتاع الـ token
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const expiry = payload.exp * 1000; // تحويل لـ milliseconds

    if (Date.now() > expiry) {
      // الـ token خلص
      localStorage.removeItem('token');
      router.navigate(['/admin/login']);
      return false;
    }

    return true;
  } catch {
    // لو الـ token تالف
    localStorage.removeItem('token');
    router.navigate(['/admin/login']);
    return false;
  }
};
