namespace Pixstock.Service.Infra
{
    /// <summary>
    /// アプリケーションコンテキストインターフェース
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// アプリケーションが使用する
        /// </summary>
        /// <returns></returns>
        string ApplicationDirectoryPath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string DatabaseDirectoryPath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ExtentionDirectoryPath { get; }
    }
}