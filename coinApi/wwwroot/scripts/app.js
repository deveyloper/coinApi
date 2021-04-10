const app = Vue.createApp({
    data() {
        return {
            apiKey: '',
            apiSecret: '',
            balances: [],
            btcTrades: [],
            usdtTrades: []
        };
    },
    mounted() {
        
    },
    methods: {
        async getBalances() {
            var url = '/api/Coin/Balances';
            var response = await axios.get(url);
            this.balances = response.data;
        },
        async deneme1() {
            console.log('deneme1');

            await this.deneme();
        },

        async getTradeForPair(currencyPair) {
            var burl = 'https://api.binance.com';
            var endPoint = '/api/v3/myTrades';
            var dataQueryString = 'timestamp=' + Date.now() + '&symbol=' + currencyPair;

            var signature = CryptoJS.HmacSHA256(dataQueryString, this.apiSecret).toString(CryptoJS.enc.Hex);
            var url = burl + endPoint + '?' + dataQueryString + '&signature=' + signature;
            var response = await axios.get(url, {
                headers: {
                    'X-MBX-APIKEY': this.apiKey
                }
            });
            return response.data;
        },

        async open(currency) {
            this.btcTrades = [];
            this.usdtTrades = [];
            this.btcTrades = await this.getTradeForPair(currency + 'BTC');
            this.usdtTrades = await this.getTradeForPair(currency + 'USDT');
        },
        increment() {
            this.test++;
        },
        decrement() {
            this.test--;
        },

        formatDate(date) {
            var d = new Date(date);
            month = '' + (d.getMonth() + 1);
            day = '' + d.getDate();
            year = d.getFullYear();
            hour = d.getHours();
            minute = d.getMinutes();
            second = d.getSeconds();

            if (month.length < 2)
                month = '0' + month;
            if (day.length < 2)
                day = '0' + day;

            return day + '.' + month + '.' + year + ' ' + hour + ':' + minute + ':' + second;
        }
    }
}).mount('#app');