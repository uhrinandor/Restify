import { TestBed } from '@angular/core/testing';

import { AdminComponent } from './admin.component';
import {provideQueryClient, QueryClient} from '@tanstack/angular-query-experimental';
import {injectCreateAdmins, injectUpdateAdmins, injectUpdateAdminPassword} from '../../api/generated/admins/admins';
import {inject} from '@angular/core';
import {AdminService} from './admin.service';

describe('Admin', () => {
  let service: AdminService;

  beforeEach(() => {
    TestBed.configureTestingModule({
        providers: [
            provideQueryClient(new QueryClient()),
        ]
    });
    service = TestBed.inject(AdminService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
