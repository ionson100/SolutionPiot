using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace piotdll
{
    public class MainPoint
    {
        public static MySettings MySettings;
        private readonly bool _useTest;

        public MainPoint(MySettings mySettings,bool useTest=false)
        {
            MySettings=mySettings;
            _useTest = useTest;
        }




        public async Task<MOut> CheckCode(List<string> codes)
        {
            MainRequestPiot requestHandler = new MainRequestPiot();
            List<MInItems> mInItems;
            if (_useTest)
            {
                mInItems = new FactoryTest().InitFactory().BuildRequest(codes.ToArray());
            }
            else
            {
                mInItems=new List<MInItems>(codes.Count);
                foreach (string code in codes)
                {
                    mInItems.Add(new MInItems
                    {
                        Km = code
                    });
                }
            }
            

            StringBuilder builder=new StringBuilder();
            MOut result = new MOut();
             await requestHandler.RequestPiotAsync(mInItems, builder, res =>
            {
                res.LogString=builder.ToString();
                result = res;
            });
            return result;

        }

    }
}
