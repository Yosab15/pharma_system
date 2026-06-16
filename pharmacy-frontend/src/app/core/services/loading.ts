import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  isLoading = signal(true);

  hide() {
    this.isLoading.set(false);
  }
}
