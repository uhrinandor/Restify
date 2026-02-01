import { TestBed } from '@angular/core/testing';

import { AdminComponent } from './admin.component';
import {provideQueryClient, QueryClient} from '@tanstack/angular-query-experimental';
import {injectCreateAdmin, injectUpdateAdmin, injectUpdateAdminPassword} from '../../api/generated/admin/admin';
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
