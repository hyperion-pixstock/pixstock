import { Injectable, EventEmitter } from '@angular/core';
import { Logger } from "angular2-logger/core";
import { IpcResponse, IpcUpdateViewResponse, IpcUpdatePropResponse } from './contract/response.contract';

/**
 * BFFからのIPCメッセージ受信と、内部イベントの発行を行うメッセージングサービスです。
 */
@Injectable()
export class MessagingService {
    /**
     * ElectronNETのRendererで使用するIPCオブジェクト
     */
    ipcRenderer: any;

    /**
     * ロガー
     */
    private logger!: Logger;

    echo: EventEmitter<string> = new EventEmitter();
    ShowContentPreview: EventEmitter<string> = new EventEmitter();
    ShowContentList: EventEmitter<string> = new EventEmitter();
    UpdateView: EventEmitter<IpcUpdateViewResponse> = new EventEmitter(); // 新API
    UpdateProp: EventEmitter<IpcUpdatePropResponse> = new EventEmitter(); // 新API

    /**
     * サービスの初期化
     *
     * @param ipcRenderer IPCオブジェクト
     * @param isRpcInitialize IPCオブジェクトのイベントハンドラ登録を行うかどうかのフラグ
     */
    initialize(_ipcRenderer: any, _isRpcInitialize: boolean, _logger: Logger) {
        this.ipcRenderer = _ipcRenderer;
        this.logger = _logger;

        let w: any = window;
        if (!w['angularComponentRef_PixstockNetService']) {
            w['angularComponentRef_PixstockNetService'] = {
                // NOTE: VFFに送信するIPCイベントをすべて登録する
                componentFn_MSG_SHOW_CONTENTPREVIEW: (event: any, arg: any) => this.onMSG_SHOW_CONTENTPREVIEW(event, arg),
                componentFn_MSG_SHOW_CONTENLIST: (event: any, arg: any) => this.onMSG_SHOW_CONTENLIST(event, arg),
                componentFn_IPC_UPDATEVIEW: (event: any, arg: any) => this.onIPC_UPDATEVIEW(event, arg),
                componentFn_IPC_UPDATEPROP: (event: any, arg: any) => this.onIPC_UPDATEPROP(event, arg)
            };
        }

        if (_isRpcInitialize) {
            _logger.info("IPCイベントの初期化");

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
        this.logger.debug("[PixstockNetService][onMSG_SHOW_CONTENTPREVIEW] : Execute");
        //this.ShowContentPreview.emit(args);
    }

    private onMSG_SHOW_CONTENLIST(event: any, args: any) {
        this.logger.debug("[PixstockNetService][onMSG_SHOW_CONTENLIST] : Execute");
        //this.ShowContentList.emit(args);
    }

    private onIPC_UPDATEVIEW(event: any, args: IpcResponse) {
        this.logger.debug("[PixstockNetService][onIPC_UPDATEVIEW] : Execute", args);

        // "IPC_UPDATEVIEW"メッセージの、本文をインスタンス化する。
        var responseObj = JSON.parse(args.body) as IpcUpdateViewResponse;

        // DUMP --------
        this.logger.debug(responseObj);
        responseObj.UpdateList.forEach(element => {
            this.logger.debug("[UpdateList] ", element);
        });
        this.logger.debug("[Parameter] ", responseObj.Parameter);
        // -------------

        this.UpdateView.emit(responseObj);
    }

    private onIPC_UPDATEPROP(event: any, args: IpcResponse) {
        this.logger.debug("[PixstockNetService][onIPC_UPDATEPROP] : Execute", args);

        // "IPC_UPDATEPROP"メッセージの、本文をインスタンス化する。
        var responseObj = JSON.parse(args.body) as IpcUpdatePropResponse;

        // DUMP
        this.logger.debug(responseObj);

        this.UpdateProp.emit(responseObj);
    }
}
