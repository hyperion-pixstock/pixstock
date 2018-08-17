import { Component, AfterViewInit } from "@angular/core";
import { ViewModel } from "ClientApp/app/viewmodel";
import { DeliveryService } from "../../../service/delivery.service";

@Component({
  selector: 'preview',
  templateUrl: './preview.screen.html',
  styleUrls: ['./preview.screen.scss']
})
export class PreviewScreen implements AfterViewInit {
  private LOGEVENT: string = "[Pixstock][PreviewScreen]";

  /**
    * コンストラクタ
    * @param viewmodel ViewModel
   */
  constructor(
    private viewmodel: ViewModel,
    private deliverySrv: DeliveryService) {

  }

  ngAfterViewInit() {
    console.debug(this.LOGEVENT, "[ngAfterViewInit]", "- IN", this.viewmodel.screenStatus.mPreviewParam);
    // プレビューコンテント更新要求メッセージを送信する

    let previewParam = this.viewmodel.screenStatus.mPreviewParam as PreviewParam
    if (previewParam.Position != undefined)
      this.deliverySrv.invalidatePreviewContentList(previewParam.Position);

    console.debug(this.LOGEVENT, "[ngAfterViewInit]", "- OUT");
  }

  /**
   * 「前へ」ボタン押下時のイベントハンドラ
   */
  onPrevPreview() {
    this.deliverySrv.invalidatePreviewContentListPrev();
  }

  /**
   * 「次へ」ボタン押下時のイベントハンドラ
   */
  onNextPreview(){
    this.deliverySrv.invalidatePreviewContentListNext();
  }
}

export interface PreviewParam {
  Position: number | null;
}
