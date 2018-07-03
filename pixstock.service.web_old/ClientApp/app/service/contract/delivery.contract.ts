export class IpcMessage {
    Body!: string;
}

export class IntentMessage {
  ServiceType!: string;
  MessageName!: string;
  Parameter!: string;
}
