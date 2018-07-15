import { Injectable, EventEmitter } from '@angular/core';
import { IpcResponse, IpcUpdateViewResponse, IpcUpdatePropResponse } from './contract/response.contract';
import { BehaviorSubject } from 'rxjs';

/**
 * BFFからのIPCメッセージ受信と、内部イベントの発行を行うメッセージングサービスです。
 */
@Injectable()
export class MessagingService {
  /**
   * ElectronNETのRendererで使用するIPCオブジェクト
   */
  ipcRenderer: any;


  echo: EventEmitter<string> = new EventEmitter();
  ShowContentPreview: EventEmitter<string> = new EventEmitter();
  ShowContentList: EventEmitter<string> = new EventEmitter();
  UpdateView: EventEmitter<IpcUpdateViewResponse> = new EventEmitter(); // 新API
  InvalidateProp: EventEmitter<IpcUpdatePropResponse> = new EventEmitter();


  // IPC_INVALIDATEPROPメッセージの内部通知用イベント
  private internalIpcUpdatePropResponse: IpcUpdatePropResponse;
  invalidateProp$ = new BehaviorSubject<IpcUpdatePropResponse>(undefined);

  get invalidateObject(): IpcUpdatePropResponse { return this.internalIpcUpdatePropResponse; }
  set invalidateObject(response: IpcUpdatePropResponse) {
    this.invalidateProp$.next(response);
    this.internalIpcUpdatePropResponse = response;
  }


  /**
   * コンストラクタ
   */
  constructor() {
    this.invalidateProp$.subscribe(this.onInvalidateProp);
  }

  /**
   * サービスの初期化
   *
   * @param ipcRenderer IPCオブジェクト
   * @param isRpcInitialize IPCオブジェクトのイベントハンドラ登録を行うかどうかのフラグ
   */
  initialize(_ipcRenderer: any, _isRpcInitialize: boolean) {
    this.ipcRenderer = _ipcRenderer;

    let w: any = window;
    if (!w['angularComponentRef_PixstockNetService']) {
      w['angularComponentRef_PixstockNetService'] = {
        // NOTE: VFFに送信するIPCイベントをすべて登録する
        componentFn_MSG_SHOW_CONTENTPREVIEW: (event: any, arg: any) => this.onMSG_SHOW_CONTENTPREVIEW(event, arg),
        componentFn_MSG_SHOW_CONTENLIST: (event: any, arg: any) => this.onMSG_SHOW_CONTENLIST(event, arg),
        componentFn_IPC_ALIVE: (event: any, arg: any) => this.onIPC_ALIVE(event, arg),
        componentFn_IPC_UPDATEVIEW: (event: any, arg: any) => this.onIPC_UPDATEVIEW(event, arg),
        componentFn_IPC_UPDATEPROP: (event: any, arg: any) => this.onIPC_UPDATEPROP(event, arg)
      };
    }

    if (_isRpcInitialize) {
      console.info("IPCイベントの初期化");

      this.ipcRenderer.removeAllListeners(["MSG_SHOW_CONTENTPREVIEW", "MSG_SHOW_CONTENLIST"]);

      this.ipcRenderer.on('MSG_SHOW_CONTENTPREVIEW', (event: any, arg: any) => {
        var ntv_window: any = window;
        ntv_window.angularComponentRef.zone.run(() => {
          ntv_window.angularComponentRef_PixstockNetService.componentFn_MSG_SHOW_CONTENTPREVIEW(event, arg);
        });
      });

      this.ipcRenderer.on('MSG_SHOW_CONTENLIST', (event: any, arg: any) => {
        var ntv_window: any = window;
        ntv_window.angularComponentRef.zone.run(() => {
          ntv_window.angularComponentRef_PixstockNetService.componentFn_MSG_SHOW_CONTENLIST(event, arg);
        });
      });

      // IPC_ALIVEメッセージ
      this.ipcRenderer.on('IPC_ALIVE', (event: any, arg: any) => {
        var ntv_window: any = window;
        ntv_window.angularComponentRef.zone.run(() => {
          ntv_window.angularComponentRef_PixstockNetService.componentFn_IPC_ALIVE(event, arg);
        });
      });

      // IPC_UPDATEVIEWメッセージ
      this.ipcRenderer.on('IPC_UPDATEVIEW', (event: any, arg: any) => {
        var ntv_window: any = window;
        ntv_window.angularComponentRef.zone.run(() => {
          ntv_window.angularComponentRef_PixstockNetService.componentFn_IPC_UPDATEVIEW(event, arg);
        });
      });

      // IPC_UPDATEPROPメッセージ
      this.ipcRenderer.on('IPC_UPDATEPROP', (event: any, arg: any) => {
        var ntv_window: any = window;
        ntv_window.angularComponentRef.zone.run(() => {
          ntv_window.angularComponentRef_PixstockNetService.componentFn_IPC_UPDATEPROP(event, arg);
        });
      });
    }
  }

  private onMSG_SHOW_CONTENTPREVIEW(event: any, args: any) {
    console.debug("[Pixstock][Messaging][onMSG_SHOW_CONTENTPREVIEW] : Execute");
    //this.ShowContentPreview.emit(args);
  }

  private onMSG_SHOW_CONTENLIST(event: any, args: any) {
    console.debug("[Pixstock][Messaging][onMSG_SHOW_CONTENLIST] : Execute");
    //this.ShowContentList.emit(args);
  }

  private onIPC_ALIVE(event: any, args: any) {
    console.debug("[Pixstock][Messaging][onIPC_ALIVE] : Execute");
  }

  private onIPC_UPDATEVIEW(event: any, args: IpcResponse) {
    console.debug("[Pixstock][Messaging][onIPC_UPDATEVIEW] : Execute", args);

    // "IPC_UPDATEVIEW"メッセージの、本文をインスタンス化する。
    var responseObj = JSON.parse(args.body) as IpcUpdateViewResponse;

    // DUMP --------
    //this.logger.debug(responseObj);
    //responseObj.UpdateList.forEach(element => {
    //    this.logger.debug("[UpdateList] ", element);
    //});
    //this.logger.debug("[Parameter] ", responseObj.Parameter);
    // -------------

    this.UpdateView.emit(responseObj);
  }

  private onIPC_UPDATEPROP(event: any, args: IpcResponse) {
    console.debug("[Pixstock][Messaging][onIPC_UPDATEPROP] : Execute", args);

    // "IPC_UPDATEPROP"メッセージの、本文をインスタンス化する。
    var responseObj = JSON.parse(args.body) as IpcUpdatePropResponse;

    // DUMP
    console.debug("[Pixstock][Messaging][onIPC_UPDATEPROP] : Dump Response", responseObj);

    //this.InvalidateProp.emit(responseObj);

    // ↑の代わりに、Rxを使ったイベント購読処理
    // イベントの購読は、
    // invalidateProp$.subscribe()で行う
    this.fireInvalidateProp(responseObj);
  }

  /*private*/ public fireInvalidateProp(eventArgs: IpcUpdatePropResponse) { // サンプルで手動で呼び出せるように、publicにしている。本来はprivate。
    this.invalidateObject = eventArgs;
  }

  onInvalidateProp(response: IpcUpdatePropResponse) {
    console.debug("[Pixstock][Messaging][onInvalidateProp] IN");
    if (response == undefined) return;

    switch (response.PropertyName) {
      case "CategoryTree":
        console.debug("[Pixstock][Messaging][onInvalidateProp] CategoryTreeプロパティ更新");

        // DEBUG: 
        break;
    }
  }
}
