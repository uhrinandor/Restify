import {inject, Injectable} from '@angular/core';
import {
    getListAdminsQueryKey,
    injectCreateAdmins, injectDeleteAdmins,
    injectListAdmins, injectUpdateAdmins, injectUpdateAdminPassword
} from '../../api/generated/admins/admins';
import {QueryClient} from '@tanstack/angular-query-experimental';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
    adminsQuery = injectListAdmins();
    createAdmin = injectCreateAdmins();
    updateAdmin = injectUpdateAdmins();
    updateAdminPassword = injectUpdateAdminPassword();
    deleteAdmin = injectDeleteAdmins();
    queryClient = inject(QueryClient);
    public invalidateListQuery = () => this.queryClient.invalidateQueries({
        queryKey: getListAdminsQueryKey(),
    })



}
