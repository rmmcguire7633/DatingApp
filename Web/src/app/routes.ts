import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/memberDetail/memberDetail.component';
import { MemberDetailResolver } from './_resolvers/memberdetail.resolver';
import { MemberListResolver } from './_resolvers/memberlist.resolver';
import { MemberEditComponent } from './members/memberEdit/memberEdit.component';
import { MemberEditResolver } from './_resolvers/memberEdit.resolver';
import { PreventUnsavedChanges } from './_guards/preventUnsavedChanges.guard';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver} },
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver} },
            { path: 'member/edit', component: MemberEditComponent, resolve: {user: MemberEditResolver}, 
                canDeactivate: [PreventUnsavedChanges] },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
