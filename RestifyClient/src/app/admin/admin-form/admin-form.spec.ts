import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminForm } from './admin-form';
import {FormControl, FormGroup, Validators} from '@angular/forms';

describe('AdminForm', () => {
  let component: AdminForm;
  let fixture: ComponentFixture<AdminForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminForm);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("form", new FormGroup({
        username: new FormControl("", Validators.required),
        accessLevel: new FormControl("", Validators.required),
    }))
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
