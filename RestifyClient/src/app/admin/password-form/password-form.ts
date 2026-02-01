import {Component, input} from '@angular/core';
import {FormGroup, ReactiveFormsModule} from "@angular/forms";

@Component({
  selector: 'app-password-form',
    imports: [
        ReactiveFormsModule
    ],
  templateUrl: './password-form.html',
  styleUrl: './password-form.css',
})
export class PasswordForm {
    form = input.required<FormGroup>();
}
