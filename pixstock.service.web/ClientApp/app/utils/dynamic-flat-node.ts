import { DeliveryService } from "ClientApp/app/service/delivery.service";
import { Category } from "ClientApp/app/model/category.model";

/** Flat node with expandable and level information */
export class DynamicFlatNode {
  /**
   * ノードが開かれてるかフラグ
   */
  opened: boolean = false;

  /**
   * コンストラクタ
   *
   * @param item
   * @param level
   * @param expandable
   * @param isLoading
   */
  constructor(
    private delivery: DeliveryService | null,
    public item: Category,
    public level = 1,
    public expandable = false,
    public isLoading = false) {
  }

  showContentList() {
    console.info("ノードボタンをクリック ", this.item.Id);
    if (this.delivery != null) {
      this.delivery.updateContentList(this.item.Id);
    }
  }
}
