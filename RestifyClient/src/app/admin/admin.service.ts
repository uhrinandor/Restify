import {inject, Injectable} from '@angular/core';
import {
    getListAdminQueryKey,
    injectCreateAdmin, injectDeleteAdmin,
    injectListAdmin, injectUpdateAdmin, injectUpdateAdminPassword
} from '../../api/generated/admin/admin';
import {QueryClient} from '@tanstack/angular-query-experimental';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
    adminsQuery = injectListAdmin();
    createAdmin = injectCreateAdmin();
    updateAdmin = injectUpdateAdmin();
    updateAdminPassword = injectUpdateAdminPassword();
    deleteAdmin = injectDeleteAdmin();
    queryClient = inject(QueryClient);
    public invalidateListQuery = () => this.queryClient.invalidateQueries({
        queryKey: getListAdminQueryKey(),
    })



}
