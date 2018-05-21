using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticRegression
{
    class lr
    {
       
        arkadasBull arkBul = new arkadasBull();     
        public double intercept = 1;   // Intercept degiskeni B0'ı temsil ediyor.
        public double temp_intercept = 1; // Temprorary Intercept güncelleme yapmak için kullandığımız ara bir geçici değişkeni ifade ediyor.
        public double stepSize = 0.00001;
        public List<double> coefficients = new List<double>(); // Sigmoid fonksiyonu için katsayılar vektörünü ifade ediyor.
        public List<double> temprorary = new List<double>(); // Güncelleme yapmak için geçici katsayılar vektörünü ifade ediyor.
        public List<double> network = new List<double>(); // Arkadas tavsiye edilecek kişinin arkadaslarinin numaralarının tutuldugu vektörü ifade ediyor.
        public List<double> nonNetwork = new List<double>(); //arkadas tavsiye edilecek kisinin arkadası olmayanların tutuldugu vektörü temsil ediyor.
        public Dictionary<double, int> friends = new Dictionary<double, int>(); // Arkadas tavsiye edilecek kisinin arkadaslarının isimleri ve '1' arkadas olma etiketinin bir arada tutuldugu HashMap yapısını ifade ediyor.
        public Dictionary<double, int> nonFriends = new Dictionary<double, int>(); // Arkadas tavsiye edilecek kisinin arkadası olmayanların isimleri ve '0' arkadas olmama etiketinin bir arada tutuldugu HashMap yapısını ifade ediyor.
        public Dictionary<double, int> trainData = new Dictionary<double, int>(); // Logistic regresyon modeli kurmak icin kullanılacak egitim verisini ifade ediyor.
        public List<double> testData = new List<double>(); // Kurulan model sonucu elde edilecek katsayılarla test edilecek test verisini ifade ediyor.
        public List<int> profil = new List<int>(); // Sorgularda her bir kisi icin olusturulacak profil bilgilerini- A0,A15- ifade ediyor.
        public void prepareData(double ogrNo)
        {
            for (int i = 0; i < 15; i++)
            {
                coefficients.Add(1);         
            }
            var friendList = arkBul.ogrenciNetwork.Where(s => s.ogrNo == ogrNo).ToList();
            var allData = arkBul.ogrenciNetwork.ToList();
            foreach (var item in friendList)
            {
                network.Add(item.ogrNo.Value);
                network.Add(item.ark0.Value);
                network.Add(item.ark1.Value);
                network.Add(item.ark2.Value);
                network.Add(item.ark3.Value);
                network.Add(item.ark4.Value);
                network.Add(item.ark5.Value);
                network.Add(item.ark6.Value);
                network.Add(item.ark7.Value);
                network.Add(item.ark8.Value);
                network.Add(item.ark9.Value);
            }         
            foreach (var item in network)
            {
                if (item != 0) {
                   friends.Add(item, 1);
                }               
            }
            foreach (var item in allData)
            {
                if(!network.Contains(item.ogrNo.Value))
                {
                    nonNetwork.Add(item.ogrNo.Value);
                }
            }
            for (int j = 0; j < nonNetwork.Count/2; j++)
            {
                nonFriends.Add(nonNetwork[j], 0);
              
            }     
            foreach (var item in friends)
            {
                trainData.Add(item.Key,item.Value);
            }
            foreach (var item in nonFriends)
            {
                trainData.Add(item.Key, item.Value);
            }
            foreach (var item in allData)
            {
                if (!trainData.ContainsKey(item.ogrNo.Value))
                {
                    testData.Add(item.ogrNo.Value);
                }         
            }            
        }
        /*prepareData metodu arkadas tavsiye edilecek kisi icin egitim ve test verisinin duzenlendigi kısımdır.Parametre olarak
         * arkadas tavsiye edilecek ogrencinin numarasını alır.İlk olarak arkadasları listesi olusturuluyor -53.satir-.Ardından arkadas olmayanlar
         bulunup yarısı * egitim datasına  diger yarısı test datasına ekleniyor.*/
        public double hBeta(double ogrNo)

        {
            profil.Clear();

            StreamReader sr = new StreamReader(@"ogrenciProfil.csv");

            while (!sr.EndOfStream)
            {
                string[] profiller = sr.ReadLine().Split(',');

                if (double.Parse(profiller[0]) == ogrNo)
                {
                    profil.Add(Int32.Parse(profiller[1]));
                    profil.Add(Int32.Parse(profiller[2]));
                    profil.Add(Int32.Parse(profiller[3]));
                    profil.Add(Int32.Parse(profiller[4]));
                    profil.Add(Int32.Parse(profiller[5]));
                    profil.Add(Int32.Parse(profiller[6]));
                    profil.Add(Int32.Parse(profiller[7]));
                    profil.Add(Int32.Parse(profiller[8]));
                    profil.Add(Int32.Parse(profiller[9]));
                    profil.Add(Int32.Parse(profiller[10]));
                    profil.Add(Int32.Parse(profiller[11]));
                    profil.Add(Int32.Parse(profiller[12]));
                    profil.Add(Int32.Parse(profiller[13]));
                    profil.Add(Int32.Parse(profiller[14]));
                    profil.Add(Int32.Parse(profiller[15]));
                }
            }

            //var profiles = arkBul.ogrenciProfil.Where(s => s.ogrNo ==ogrNo).ToList();


            //    foreach (var item in profiles)
            //    {
            //        profil.Add(item.a0.Value);
            //        profil.Add(item.a1.Value);
            //        profil.Add(item.a2.Value);
            //        profil.Add(item.a3.Value);
            //        profil.Add(item.a4.Value);
            //        profil.Add(item.a5.Value);
            //        profil.Add(item.a6.Value);
            //        profil.Add(item.a7.Value);
            //        profil.Add(item.a8.Value);
            //        profil.Add(item.a9.Value);
            //        profil.Add(item.a10.Value);
            //        profil.Add(item.a11.Value);
            //        profil.Add(item.a12.Value);
            //        profil.Add(item.a13.Value);
            //        profil.Add(item.a14.Value);

            //    }


            return 1 / (1 + Math.Pow(Math.E,                
                (-1 * (intercept + 
                (coefficients[0] * profil[0]) +
                (coefficients[1] * profil[1]) + 
                (coefficients[2] * profil[2]) +
                (coefficients[3] * profil[3]) +
                (coefficients[4] * profil[4]) +
                (coefficients[5] * profil[5]) +
                (coefficients[6] * profil[6]) +
                (coefficients[7] * profil[7]) +
                (coefficients[8] * profil[8]) +
                (coefficients[9] * profil[9]) +
                (coefficients[10] * profil[10]) +
                (coefficients[11] * profil[11]) +
                (coefficients[12] * profil[12]) +
                (coefficients[13] * profil[13]) +
                (coefficients[14] * profil[14]))))); 
            
        }
        /*hBeta metodu matematikte sigmoid ' 1/1+e^(-b0+b1x1+....+bnxn) ' olarak bilinen fonksiyondur. hBeta metodunda sigmoid fonksiyonunun 0-1 arasında degerler almasından faydalanarak
         kisiler arasında % 'likli olarak arkadas tavsiye etmesine olanak saglamaktadır.Burada yapılan islem  x1-xn bagımsız degiskenleri ile b0-b15 katsayılar vektörünün ilgili elemanlarının
         toplanmasıdır.Bu toplamın sonucu yuksek cıktıkca sigmoid fonksiyonunun sonucu da 1 e yaklasir.Kodun aciklamasi ise fonksiyona giren her bir ogrenci icin profil bilgileri-x1,xn- 
         sorgulanir-104.satir -.Ardindan sigmoid fonksiyonuna sokulur. */
        public void getCoefficients() {
            double c = 0;
            
            List<double> numbers = new List<double>();
            List<int> labels = new List<int>();
           // double temp_inter;

            foreach (var item in trainData)
            {
                numbers.Add(item.Key);
                labels.Add(item.Value);
            }

            
            for (int i = 0; i < 100; i++)
            {
                temp_intercept = 1;

                for (int f = 0; f < 15; f++)
                {
                    temprorary.Add(1);
                }

                for (int k = 0; k < numbers.Count; k++)
                {
                   c+=(hBeta(numbers[k]) - labels[k]);
                    
                }
                c /= trainData.Count;
                temp_intercept = intercept - stepSize * c;
                //intercept = intercept - stepSize * c;

                    for (int j = 0; j < 15; j++)
                    {

                    for (int q = 0; q < numbers.Count; q++)
                    {

                      c+=(((hBeta(numbers[q]) - labels[q]) * profil[j]));
                    }
                   c /= trainData.Count;
                    temprorary[j] = coefficients[j] - (stepSize * c );
                   // coefficients[j] = coefficients[j] - stepSize * (c * profil[j]);
                }

                coefficients = temprorary;
               
                intercept = temp_intercept;

            }    

                
            
        }
        /*getCofficient metodu icerisinde egitim verisine gore sigmoid fonksiyonuna girecek katsayi degerlerimiz surekli olarak guncellenir.
         Bu degerler doğrultusunda test verisi icin nihai katsayılar vektoru bulunur. Metodumuzun icerisinde ise her bir ogrenci icin 100 iterasyonluk bir dongu 
         olusturulur.-165. satir.- 173. satirdan itibaren b0 guncellenmeye baslanir. 185. satirdan itibarense diger katsayilar guncellenmeye baslanir.  */
        public Dictionary<double,string> test(List<double> testData) {

            Dictionary<double, string> sonuc = new Dictionary<double, string>();

            Dictionary<double, double> result = new Dictionary<double, double>();

            foreach (var item in testData)
            {

                result.Add(item, hBeta(item));

            }
            


            List<KeyValuePair<double,double>> sirali = result.ToList();

            sirali.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

           // result.OrderBy(value => value.Value);

            for (int m =testData.Count-1; m >testData.Count-11; m--)
            {
                double sayi = sirali[m].Key;
                var ogrIsim = arkBul.ogrenciIsim.Where(s => s.ogrNo == sayi).FirstOrDefault();
                sonuc.Add(ogrIsim.ogrNo.Value, ogrIsim.isim);

                
            }
 
            return sonuc;

        }
        /*Son olarak test metodumuzda ise buldugumuz katsayılara gore test verimizdeki her bir ogrenci sigmoid fonksiyonuna sokulur. Buradan cıkan sonuclar
         bir hashMap yapısı icerisinde ogrenci numarası sonuc seklinde tutulur.Sonuclara gore sıralama işlemi yapıldıktan sonra elde edilen sonuclar bir matrise kaydedilir.*/

    }
}
