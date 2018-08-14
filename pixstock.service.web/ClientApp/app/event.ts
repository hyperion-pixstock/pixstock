/**
 * 項目一覧での選択アイテムイベントのパラメータ
 */
export class ItemListSelectEventArg {
  /**
   *
   * @param _sender
   * @param _item 項目一覧内のアイテム位置
   * @param _position 選択アイテム
   */
  constructor(private _sender: object, private _item: object, private _position: number) {
  }

  get sender() { return this._sender; }
  get item() { return this._item; }
  get position() { return this._position; }

}
