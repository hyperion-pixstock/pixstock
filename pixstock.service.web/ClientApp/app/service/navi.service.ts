import { Injectable } from "@angular/core";
import { IpcUpdateViewResponse } from "./contract/response.contract";
import { CourierService } from "./courier.service";
import { ViewModel } from "../viewmodel";

@Injectable()
export class NaviService {
  private LOGEVENT: string = "[Pixstock][NaviService]";

  /**
   * コンストラクタ
   *
   * @param courierSrv
   * @param viewModel
   */
  constructor(
    private courierSrv: CourierService,
    private viewModel: ViewModel
  ) {
    this.courierSrv.updateView$.subscribe((response: IpcUpdateViewResponse) => {
      if (response == undefined) return;

      console.debug(this.LOGEVENT, "[UpdateView$] - IN");
      console.debug(this.LOGEVENT, "[UpdateView$] レスポンス:", response);

      let screenName = response.NextScreenName;
      console.debug(this.LOGEVENT, "[UpdateView$] ScrrenName:", screenName);

      switch (screenName) {
        case "Dashboard":
          this.viewModel.screenStatus.showDashboard();
          break;
        case "Finder":
          this.viewModel.screenStatus.showFinder();
          break;
        case "Preview":
          this.viewModel.screenStatus.showPreview({});
          break;
        case "ContentListPreview":
          this.viewModel.screenStatus.showPreview({
            Position: parseInt(String(response.Parameter))
          });
          break;
        default:
          console.warn(this.LOGEVENT, "[UpdateView$] 未定義の画面名", screenName);
          break;
      }

      console.debug(this.LOGEVENT, "[UpdateView$] - OUT");
    });
  }
}
