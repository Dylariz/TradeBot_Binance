# TradeBot_Binance
This is a simple assistant bot for tracking price changes on Binance and broadcasting changes to Discord.

## Preparations
1. Create a new API key at your Binance account.
2. Create a new webhook in the Discord channel that is convenient for you.
3. Download the [latest release](https://github.com/Dylariz/TradeBot_Binance/releases/tag/Releases) of TradeBot and unzip it.
4. Change `start.bat` as follows:
``` bash
TradeBot_Binance.exe "your ApiKey" "your SecretKey" "your Discord WebHook URL" "optional: relative Сryptocurrency (default: USDT)"
```
5. Start `start.bat`.

## About
- This program will help you track the Binance market relative to some cryptocurrency `(default: USDT)`. 
- When any cryptocurrency increases by `3%` in one minute relative to the specified cryptocurrency, it will be displayed in the console. 
- However, if it increases by `6%`, the program will send a notification to Discord.
