import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { User } from '../../shared/models/User';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl: string = environment.hubUrl;

  onlineUsers = signal<number[]>([]);

  private hubConnection?: HubConnection;

  private toastr = inject(ToastrService);

  private router = inject(Router);


  createHubConnection(user: User) {

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('NewOnlineUser', (userId) => {
      this.onlineUsers.set([...this.onlineUsers(), userId])
      this.toastr.info("new online user " + userId);
    });

    this.hubConnection.on('OnlineUsersList', (userIDs: number[]) => {
      this.onlineUsers.set(userIDs)

    })

    this.hubConnection.on('UserIsOffline', (offlineUserId: number) => {
      this.onlineUsers.set([...this.onlineUsers().filter(onlineUserId => onlineUserId !== offlineUserId)]);
      this.toastr.info("User is Dissconnected " + offlineUserId);
    })

    this.hubConnection.on('NewMessageReceived', ({ userName, userId }) => {
      this.toastr.info(userName + ' has sent to you a message');
      // later...
      // 1- when tap on message, go to contact.
      // 2- increase the counter of messages.
    })
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
