import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {Navitem} from '../components/navitem/navitem';
import {ToastrModule} from 'ngx-toastr';

@Component({
  selector: 'app-root',
    imports: [RouterOutlet, Navitem],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  public readonly svgClass = "my-1.5 inline-block size-5";
}
