import {inject, Injectable} from '@angular/core';
import {
    getListAdminsQueryKey,
    injectCreateAdmin, injectDeleteAdmin,
    injectListAdmins, injectUpdateAdmin, injectUpdateAdminPassword
} from '../../api/generated/admin/admin';
import {QueryClient} from '@tanstack/angular-query-experimental';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
    adminsQuery = injectListAdmins();
    createAdmin = injectCreateAdmin();
    updateAdmin = injectUpdateAdmin();
    updateAdminPassword = injectUpdateAdminPassword();
    deleteAdmin = injectDeleteAdmin();
    queryClient = inject(QueryClient);
    public invalidateListQuery = () => this.queryClient.invalidateQueries({
        queryKey: getListAdminsQueryKey(),
    })



}
