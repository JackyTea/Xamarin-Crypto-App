﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace JTea_DPS926_Assignment2
{
    public class DatabaseManager
    {
        // connection instance
        SQLiteAsyncConnection _connection;

        // api networking instance
        NetworkingManager api = new NetworkingManager();

        // constructor (0 params required)
        public DatabaseManager()
        {
            _connection = DependencyService.Get<SQLiteDBInterface>().createSQLiteDB();
        }

        // get missing data from api to fill out favourites
        public async void ComepleteCoinData(ObservableCollection<Coin> coinsCollection)
        {
            foreach (Coin c in coinsCollection)
            {
                Coin fetchedCoin = await api.getOneCoin(c.id);
                c.image = fetchedCoin.image;
                c.description = fetchedCoin.description;
                c.market_data = fetchedCoin.market_data;
            }
        }

        // get all records from table
        public async Task<ObservableCollection<Coin>> QueryAllCoins()
        {
            await _connection.CreateTableAsync<Coin>();
            var coinsFromDB = await _connection.Table<Coin>().ToListAsync();
            var coinsCollection = new ObservableCollection<Coin>(coinsFromDB);
            ComepleteCoinData(coinsCollection);
            return coinsCollection;
        }

        // check if coin is in favourites
        public async Task<bool> CoinExists(string id)
        {
            bool exists = false;
            try
            {
                exists = await _connection.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Coin WHERE id=?)", id);
            }
            catch
            {
                exists = false;
            }
            return exists;
        }

        // insert into database
        public async void InsertCoin(Coin newCoin)
        {
            await _connection.InsertAsync(newCoin);
        }

        // update record in database
        public async void UpdateCoin(Coin updateCoin)
        {
            await _connection.UpdateAsync(updateCoin);
        }

        // delete from database
        public async void DeleteCoin(Coin removeCoin)
        {
            await _connection.DeleteAsync(removeCoin);
        }
    }
}
