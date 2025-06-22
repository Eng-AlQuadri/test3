import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Contact } from '../../shared/models/Contact';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/User';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Message } from '../../shared/models/Message';
import { Group } from '../../shared/models/Group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  private http = inject(HttpClient);

  private hubConnection?: HubConnection;

  messages = signal<Message[]>([]);

  baseUrl: string = environment.apiUrl;

  hubUrl: string = environment.hubUrl;

  loadContacts() {
    return this.http.get<Contact[]>(this.baseUrl + 'students/contacts');
  }

  createHubConnection(user: User, otherUserId: number) {

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'dual-messaging?user=' + otherUserId, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messages.set(messages);
    })

    this.hubConnection.on('NewMessage', message => {
      this.messages.set([...this.messages(), message]);
    })

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(x => x.userId === otherUserId)) {
        this.messages().forEach(message => {
          if (!message.dateRead) {
            message.dateRead = new Date(Date.now());
          }
        });
        this.messages.set([...this.messages()])
      }
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  async sendMessage(userId: string, content: string) {

    const message = { recipientId: userId, content }

    return this.hubConnection?.invoke("SendMessage", message)
      .catch(error => console.log(error));
  }
}
