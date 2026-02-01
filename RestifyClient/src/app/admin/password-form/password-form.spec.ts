import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasswordForm } from './password-form';
import {FormControl, FormGroup, Validators} from '@angular/forms';

describe('PasswordForm', () => {
  let component: PasswordForm;
  let fixture: ComponentFixture<PasswordForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasswordForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PasswordForm);
    fixture.componentRef.setInput("form", new FormGroup({
        oldPassword: new FormControl("", Validators.required),
        newPassword: new FormControl("", Validators.required),
    }));
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
