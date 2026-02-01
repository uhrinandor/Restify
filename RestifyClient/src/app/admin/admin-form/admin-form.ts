import {Component, effect, input} from '@angular/core';
import {FormGroup, ReactiveFormsModule} from '@angular/forms';
import { Admin } from '../../../api/model';

@Component({
  selector: 'app-admin-form',
    imports: [
        ReactiveFormsModule
    ],
  templateUrl: './admin-form.html'
})
export class AdminForm {
    admin = input<Admin>();
    password = input<boolean>();
    form = input.required<FormGroup>();

    constructor() {
        effect(() => {
            const data = this.admin();
            if (data) {
                this.form().patchValue({
                    username: data.username,
                    accessLevel: data.accessLevel
                });
            }else{
                this.form().reset();
            }
        });
    }
}
