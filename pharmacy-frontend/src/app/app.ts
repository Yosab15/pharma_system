import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppLoader } from './shared/app-loader/app-loader';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,AppLoader],
  templateUrl: './app.html'
})
export class App {

}
