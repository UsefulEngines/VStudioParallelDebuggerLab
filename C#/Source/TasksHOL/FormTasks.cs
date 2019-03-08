using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HOL
{
    public class Work
    {
        public static CancellationTokenSource CancelToken = new CancellationTokenSource();

        static Random rnd = new Random();
        static ManualResetEvent evt1 = new ManualResetEvent(false);
        static object lock1 = new object(), lock2 = new object();

        private static int order = 0;
        public static void ProcessForm(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "ProcessForm";

            form.OrderDate = DateTime.Now;
            int o = Interlocked.Increment(ref order);
            form.OrderId = o;

            PerformEligibilityCheck(f);

            if (form.CreditScore > 500 && form.Eligible)
            {
                FulfillOrder(f);
            }
            else SendCancelLetter(f);

            // Update form with final disposition
            Console.WriteLine("Completed: {0}", form.Name);
        }

        #region "Eligibility"
        public static void PerformEligibilityCheck(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "PerformEligibilityCheck";

            LookupServiceArea(form);
            PerformCreditCheck(form);
        }

        public static void LookupServiceArea(OrderForm form)
        {
            form.OrderStatus = "LookupServiceArea";
            GeocodeAddress(form);
        }

        public static void GeocodeAddress(OrderForm form)
        {
            form.OrderStatus = "GeocodeAddress";
            CallAddressProvider(form);
            form.Eligible = true;
        }

        static int bb = 0;
        public static void CallAddressProvider(OrderForm form)
        {
            form.OrderStatus = "CallAddressProvider";

            int a = Interlocked.Increment(ref bb);

            if (a == 5)
            {
                // First break
                Debugger.Break();
                evt1.Set();
            }
            else
            {
                switch (form.OrderId)
                {
                    case 2:
                        lock (lock2)
                        {
                            evt2.WaitOne();
                            lock (lock1)
                            {
                                // Simulate protected operation here...
                                while (!CancelToken.IsCancellationRequested) Thread.Sleep(1000);
                            }
                        }
                        break;
                    case 4:
                        lock (lock1)
                        {
                            evt2.WaitOne();
                            lock (lock2)
                            {
                                // Simulate protected operation here...
                                while (!CancelToken.IsCancellationRequested) Thread.Sleep(1000);
                            }
                        }
                        break;
                    default:
                        
                        evt1.WaitOne();
                        break;
                }
            }
        }
        #endregion

        #region "Credit check"
        public static void PerformCreditCheck(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "PerformCreditCheck";
            ObtainCreditReport(form);
        }

        public static void ObtainCreditReport(OrderForm form)
        {
            form.OrderStatus = "ObtainCreditReport";
            CreditReportHelper(form);

            form.CreditScore = (form.OrderId == 1) ? 250 : 700;
        }

        static int cc = 0;
        static ManualResetEvent evt2 = new ManualResetEvent(false);
        public static void CreditReportHelper(OrderForm form)
        {
            form.OrderStatus = "CreditReportHelper";
            int a = Interlocked.Increment(ref cc);

            if (a == 3)
            {
                // Second break
                Debugger.Break();

                evt2.Set();
            }
            else
            {
                evt2.WaitOne();
                if (a == 1)
                    CacheCreditInfo(form);
            }
        }

        public static void CacheCreditInfo(OrderForm form)
        {
            form.OrderStatus = "CacheCreditInfo";
            UpdateDatabase(form);
        }

        public static void UpdateDatabase(OrderForm form)
        {
            form.OrderStatus = "UpdateDatabase";
            evt3.WaitOne();
        }
        #endregion

        #region "Cancel"
        public static void SendCancelLetter(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "SendCancelLetter";
            GenerateLetter(f);
            form.CancelDate = DateTime.Now;
        }

        private static void GenerateLetter(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "GenerateLetter";
            PerformAddressCorrection(f);
        }

        public static void PerformAddressCorrection(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "PerformAddressCorrection";

            evt3.WaitOne();

            form.AddressVerified = true;
        }
        #endregion

        #region "Fulfillment"
        public static void FulfillOrder(object f)
        {
            OrderForm form = f as OrderForm;
            form.OrderStatus = "FulfillOrder";
            InitiateBilling(f);

            // Simulate some work...
            form.ShipDate = DateTime.Now;
        }
        #endregion

        #region "Billing"
        public static void InitiateBilling(object o)
        {
            OrderForm form = o as OrderForm;
            form.OrderStatus = "InitiateBilling";

            // Simulate some work (credit card processing)...
            CreditCardAuth(form);
        }

        private static void CreditCardAuth(OrderForm form)
        {
            form.OrderStatus = "CreditCardAuth";
            CreditCardAuthHelper(form);
        }

        private static int auth = 0;
        static ManualResetEvent evt3 = new ManualResetEvent(false);

        private static void CreditCardAuthHelper(OrderForm form)
        {
            form.OrderStatus = "CreditCardAuthHelper";
            int a = Interlocked.Increment(ref auth);
            form.PaymentAuthorization = a.ToString();

            // Thread-safe comparison so only one thread sees it as true
            if (a == 1)
            {
                evt3.Set();
            }
            else evt3.WaitOne();

            Task.WaitAll(MainWindow.tasks.ToArray());
            
        }

        #endregion

    }
}
