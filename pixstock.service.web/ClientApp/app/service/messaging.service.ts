import { Injectable, EventEmitter } from '@angular/core';
import { IpcResponse, IpcUpdateViewResponse, IpcUpdatePropResponse } from './contract/response.contract';
import { BehaviorSubject } from 'rxjs';
import { CourierService } from './courier.service';

/**
 * BFFからのIPCメッセージ受信と、内部イベントの発行を行うメッセージングサービスです。
 */
@Injectable()
export class MessagingService {
  /**
   * ElectronNETのRendererで使用するIPCオブジェクト
   */
  ipcRenderer: any;

  UpdateView: EventEmitter<IpcUpdateViewResponse> = new EventEmitter();  // TODO: CourierServiceに移動する

  /**
   * コンストラクタ
   */
  constructor(private courier:CourierService) {

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
     // TODO: IPC_UPDATEPROPと同様に、Courierを使用してイベントを発火する
  }

  private onMSG_SHOW_CONTENLIST(event: any, args: any) {
    console.debug("[Pixstock][Messaging][onMSG_SHOW_CONTENLIST] : Execute");
     // TODO: IPC_UPDATEPROPと同様に、Courierを使用してイベントを発火する
  }

  private onIPC_ALIVE(event: any, args: any) {
    console.debug("[Pixstock][Messaging][onIPC_ALIVE] : Execute");
     // TODO: IPC_UPDATEPROPと同様に、Courierを使用してイベントを発火する
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

    this.UpdateView.emit(responseObj); // TODO: IPC_UPDATEPROPと同様に、Courierを使用してイベントを発火する
  }

  private onIPC_UPDATEPROP(event: any, args: IpcResponse) {
    console.debug("[Pixstock][Messaging][onIPC_UPDATEPROP] : Execute", args);

    // "IPC_UPDATEPROP"メッセージの、本文をインスタンス化する。
    var responseObj = JSON.parse(args.body) as IpcUpdatePropResponse;
    // DUMP: JSON文字列からオブジェクトを作成できているか確認用のダンプ出力
    console.debug("[Pixstock][Messaging][onIPC_UPDATEPROP] : Dump Response", responseObj);

    this.courier.fireInvalidateProp(responseObj);
  }
}
