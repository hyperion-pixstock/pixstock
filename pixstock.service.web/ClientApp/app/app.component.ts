import { Component } from '@angular/core';
import { IpcUpdatePropResponse, IpcUpdateViewResponse, UpdateViewRequestItem } from './service/contract/response.contract';
import { DeliveryService } from './service/delivery.service';
import { MessagingService } from './service/messaging.service';
import { ViewModel } from './viewmodel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'Asp Net Core 2.1 Angular 6 Template';

  constructor(
    public messaging: MessagingService,
    private delivery: DeliveryService,
    private viewModel: ViewModel
  ) {

  }

  // サンプル
  // ボタン押下で、InvalidatePropイベントを発生させる
  onDebugBasicButton() {
    console.info("onDebugBasicButton");
    let obj: IpcUpdatePropResponse = {
      PropertyName: "TEST Propety Name",
      Hint: "HINT",
      Value: "VALUE"
    };
    //this.messaging.fireInvalidateProp(obj); // このデバッグコマンドは無効化しました
  }

  /**
   * サンプル
   */
  onTRNS_TOPSCREEN() {
    console.info("onTRNS_TOPSCREEN");

    this.delivery.transTopScreen();
  }

  /**
   * サンプル
   */
  onACT_REQINVALIDATE_CATEGORYTREE() {
    console.info("onACT_REQINVALIDATE_CATEGORYTREE");
    this.delivery.updateCategoryTree(1);
  }

  /**
   * (Debug)ファインダ画面へ遷移する
   */
  onTrnsFinder() {
    this.delivery.showFinder();
  }

  /**
   * サンプル
   */
  onDebugBasicButton2() {
    console.info("onDebugBasicButton2");

    this.delivery.executeDebugCommand("Nanikaなにか");
  }

  /**
   * (DEBUG)ルート状態に強制遷移します
   */
  onDebugTransrationRoot() {
    this.delivery.transRootBack();
  }

}
