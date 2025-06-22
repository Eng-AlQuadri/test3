import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MessageService } from '../../../../core/services/message.service';
import { Contact } from '../../../models/Contact';
import { RouterModule } from '@angular/router';
import { AccountService } from '../../../../core/services/account.service';
import { User } from '../../../models/User';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-messages',
  imports: [
    RouterModule,
    FormsModule,
    CommonModule
  ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})

export class MessagesComponent implements OnInit, OnDestroy{

  @ViewChild('messageForm') messageForm?: NgForm;

  accountService = inject(AccountService);

  messageService = inject(MessageService);

  contacts: Contact[] = [];

  currentUser?: User;

  selectedContact?: Contact;

  messageContent?: string;

  ngOnInit(): void {
    this.loadContacts();
    this.currentUser = this.accountService.currentUser()!;
  }

  ngOnDestroy(): void {
    this.stopHubConnection();
  }

  loadContacts() {
    this.messageService.loadContacts().subscribe(response => {
      this.contacts = response;
    })
  }

  onSelectContact(contact: Contact) {
    this.stopHubConnection();
    this.selectedContact = contact;
    this.createHubConnection(this.currentUser!, contact.contactId);
  }

  createHubConnection(currentUser: User, otherUserId: number) {
    this.messageService.createHubConnection(currentUser, otherUserId);
  }

  stopHubConnection() {
    this.messageService.stopHubConnection();
  }

  sendMessage() {
    if (this.messageContent) {
      this.messageService.sendMessage(this.selectedContact?.contactId!.toString()!, this.messageContent).then(() => {
        this.messageForm?.reset();
      })
    }
  }
}
