import {Component, effect, inject} from '@angular/core';
import {AdminService} from './admin.service';
import {DatePipe} from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {AdminForm} from './admin-form/admin-form';
import {getListAdminsQueryKey} from '../../api/generated/admins/admins';
import {QueryClient} from '@tanstack/angular-query-experimental';
import {PasswordForm} from './password-form/password-form';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-admin',
    imports: [
        DatePipe,
        ReactiveFormsModule,
        AdminForm,
        PasswordForm
    ],
  templateUrl: './admin.component.html'
})
export class AdminComponent {
    public adminService = inject(AdminService);
    toastr = inject(ToastrService);
    protected readonly Date = Date;

    selectedAdminId: string | null = null;

    updateAdminForm = new FormGroup({
        username: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(4)]}),
        accessLevel: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    });

    createAdminForm = new FormGroup({
        username: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(4)]}),
        accessLevel: new FormControl(0, { nonNullable: true }),
        password: new FormControl('', { nonNullable: true })
    });

    changePasswordForm = new FormGroup({
        oldPassword: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(6)]}),
        newPassword: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(6)]}),
    })

    onUpdateSubmit(){
        if(this.updateAdminForm.valid && this.selectedAdmin){
            const data = this.updateAdminForm.getRawValue();
            this.adminService.updateAdmin.mutate({
                id: this.selectedAdminId!,
                data: {
                    username: data.username,
                    writeMode: data.accessLevel == 1,
                }
            }, this.handleMutation(
                {
                    successMsg: `Successful update!`,
                    modalId: 'modify-modal',
                    form: this.updateAdminForm,
                    title: `Update admin: ${this.selectedAdmin?.username}`
                }
            ))
        }
    }

    onCreateSubmit(){
        if(this.createAdminForm.valid){
            const data = this.createAdminForm.getRawValue();
            this.adminService.createAdmin.mutate({
                data: {
                    username: data.username,
                    writeMode: data.accessLevel == 1,
                    password: data.password,
                }
            }, this.handleMutation(
                {
                    successMsg: `Successful create!`,
                    modalId: 'create-modal',
                    form: this.createAdminForm,
                    title: `Create admin: ${data.username}`
                }
            ))
        }
    }

    onChangePasswordSubmit(){
        if(this.selectedAdmin && this.changePasswordForm.valid){
            const data = this.changePasswordForm.getRawValue();
            this.adminService.updateAdminPassword.mutate({
                id: this.selectedAdminId!,
                data: {
                    oldPassword: data.oldPassword,
                    newPassword: data.newPassword,
                }
            }, this.handleMutation(
                {
                    successMsg: `Successful password change!`,
                    modalId: 'password-modal',
                    form: this.changePasswordForm,
                    title: `Change password: ${this.selectedAdmin?.username}`
                }
            ))
        }
    }

    onDelete(){
        if(this.selectedAdmin && confirm("Confirm to delete?")){
            this.adminService.deleteAdmin.mutate({
                id: this.selectedAdminId!,
            }, this.handleMutation(
                {
                    modalId: '',
                    successMsg: `Successfully deleted admin!`,
                    title: `Delete Admin: ${this.selectedAdmin?.username}`
                }
            ))
        }
    }
    get selectedAdmin() {
        return this.adminService.adminsQuery.data()?.find(a => a.id === this.selectedAdminId);
    }

    toggleSelection(id: string) {
        this.selectedAdminId = this.selectedAdminId === id ? null : id;
    }

    private closeModal(id: string) {
        const checkbox = document.getElementById(id) as HTMLInputElement;
        if (checkbox) checkbox.checked = false;
    }

    private handleMutation(options: {
        modalId: string,
        form?: FormGroup,
        successMsg: string,
        title: string
    }) {
        return {
            onSuccess: () => {
                this.closeModal(options.modalId);
                this.adminService.invalidateListQuery();
                this.toastr.success(options.successMsg, options.title);
                options.form?.reset();
            },
            onError: () => {
                this.toastr.error(`Something went wrong!`, options.title);
            }
        };
    }
}
