import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminComponent } from './admin.component';
import {DatePipe} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import {AdminForm} from './admin-form/admin-form';
import {PasswordForm} from './password-form/password-form';
import {AdminService} from './admin.service';
import {QueryClient} from '@tanstack/angular-query-experimental';
import {ToastrModule} from 'ngx-toastr';

describe('Admin', () => {
  let component: AdminComponent;
  let fixture: ComponentFixture<AdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminComponent,         DatePipe,
          ReactiveFormsModule,
          AdminForm,
          PasswordForm, ToastrModule.forRoot()],
        providers: [QueryClient, AdminService],
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
