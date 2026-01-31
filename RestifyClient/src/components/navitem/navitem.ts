import {Component, input} from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nav-item',
    imports: [
        RouterLink
    ],
    templateUrl: './navitem.html',
    styleUrl: './navitem.css'
})
export class Navitem {
    label = input.required<string>();
    route = input.required<string>();
    tip = input<string>();
    class = input<string>('');
}
