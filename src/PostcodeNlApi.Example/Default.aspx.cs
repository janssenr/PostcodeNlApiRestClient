using System;
using System.Linq;
using System.Web.Configuration;
using PostcodeNlApi;
using PostcodeNlApi.Signal;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlResult.Visible = false;
        pnlAddressResponse.Visible = false;
        pnlSignalResponse.Visible = false;
        pnlError.Visible = false;
        pnlTimeTaken.Visible = false;
        pnlRawRequestResponse.Visible = false;
        if (!IsPostBack)
        {
            txtApiUrl.Text = "https://api.postcode.nl/rest";
            txtKey.Text = WebConfigurationManager.AppSettings["appKey"];
            txtSecret.Text = WebConfigurationManager.AppSettings["appSecret"];
            pnlAddress.Visible = false;
            pnlSignal.Visible = false;
            BindData();
        }
    }

    private void BindData()
    {
        ddlTransactionStatus.Items.Add("new"); // Transaction is currently being 'built', ie someone is shopping
        ddlTransactionStatus.Items.Add("new-checkout"); // Transaction is currently being 'built', and customer is in 'checkout' step
        ddlTransactionStatus.Items.Add("pending"); // Transaction has been agreed upon by customer, but customer needs to finish some agreed upon actions (unknown actions)
        ddlTransactionStatus.Items.Add("pending-payment"); // Transaction has been agreed upon by customer, but customer needs to finish external payment
        ddlTransactionStatus.Items.Add("processing"); // Transaction has been agreed upon and payment is 'completed', shop needs to package & ship order
        ddlTransactionStatus.Items.Add("complete"); // Transaction has been shipped (but not necessarily delivered)
        ddlTransactionStatus.Items.Add("closed"); // Transaction has been shipped, and (assumed) delivered
        ddlTransactionStatus.Items.Add("cancelled"); // Transaction has been cancelled from any state. Order will not continue and will not be revived later
        ddlTransactionStatus.Items.Add("cancelled-by-customer"); // Transaction has been cancelled by customer, shop needs to reverse any payments made, if necessary
        ddlTransactionStatus.Items.Add("cancelled-by-shop"); // Transaction has been cancelled by shop, shop needs to reverse any payments made, if necessary
        ddlTransactionStatus.Items.Add("onhold"); // Transaction needs some custom interaction by customer or shop before it can continue.
        ddlTransactionStatus.Items.Add("other"); // Another status not listed here
    }

    protected void btnAddressSend_Click(object sender, EventArgs e)
    {
        DateTime start = DateTime.Now;

        var client = new PostcodeNlApiRestClient(txtKey.Text, txtSecret.Text, txtApiUrl.Text);
        try
        {
            if (chkAddressShowRawRequestResponse.Checked)
                client.SetDebugEnabled();

            var result = client.LookupAddress(txtPostcode.Text, txtHouseNumber.Text, txtHouseNumberAddition.Text,
                chkValidateHouseNumberAddition.Checked);

            lStreet.Text = result.Street;
            lHouseNumber.Text = result.HouseNumber;
            lHouseNumberAddition.Text = !string.IsNullOrEmpty(result.HouseNumberAddition) ? result.HouseNumberAddition : txtHouseNumberAddition.Text;
            lPostcode.Text = result.Postcode;
            lCity.Text = result.City;
            lResult.Text = result.ToString();
            pnlResult.Visible = true;
            pnlAddressResponse.Visible = true;
        }
        catch (PostcodeNlApiRestClientClientException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Client error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientServiceException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Service error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientInputInvalidException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Input error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientAuthenticationException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Authentication error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientAddressNotFoundException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Address not found";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (Exception ex)
        {
            pnlError.Visible = true;
            lType.Text = "Error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }

        if (chkAddressShowRawRequestResponse.Checked)
        {
            pnlRawRequestResponse.Visible = true;
            var debugData = client.GetDebugData();
            if (debugData != null)
            {
                lRequest.Text = debugData["request"];
                lResponse.Text = debugData["response"];
            }
        }
        pnlTimeTaken.Visible = true;
        lTimeTaken.Text = DateTime.Now.Subtract(start).TotalSeconds.ToString();
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        pnlAddress.Visible = false;
        pnlSignal.Visible = false;
        switch (ddlService.SelectedValue)
        {
            case "Address":
                pnlAddress.Visible = true;
                break;
            case "Signal":
                pnlSignal.Visible = true;
                break;
        }
    }
    protected void btnSignalSend_Click(object sender, EventArgs e)
    {
        DateTime start = DateTime.Now;

        var client = new PostcodeNlApiRestClient(txtKey.Text, txtSecret.Text, txtApiUrl.Text);
        try
        {
            if (chkSignalShowRawRequestResponse.Checked)
                client.SetDebugEnabled();

            DateTime birthDate;
            DateTime time;
            int houseNumber;
            double cost;
            int weight;

            var request = new PostcodeNlSignalRequest
            {
                Customer =
                    new PostcodeNlSignalCustomer
                    {
                        FirstName = txtCustomerFirstName.Text.NullIfEmpty(),
                        LastName = txtCustomerLastName.Text.NullIfEmpty(),
                        BirthDate =
                            DateTime.TryParse(txtCustomerBirthDate.Text, out birthDate) ? (DateTime?)birthDate : null,
                        Email = txtCustomerEmail.Text.NullIfEmpty(),
                        EmailDomain = txtCustomerEmailDomain.Text.NullIfEmpty(),
                        PhoneNumber = txtCustomerPhoneNumber.Text.NullIfEmpty(),
                        BankNumber = txtCustomerBankNumber.Text.NullIfEmpty(),
                        Site = txtCustomerSite.Text.NullIfEmpty(),
                        InternalId = txtCustomerInternalId.Text.NullIfEmpty(),
                        Address =
                            new PostcodeNlSignalAddress
                            {
                                Postcode = txtCustomerAddressPostcode.Text.NullIfEmpty(),
                                HouseNumber =
                                    int.TryParse(txtCustomerAddressHouseNumber.Text, out houseNumber)
                                        ? (int?)houseNumber
                                        : null,
                                HouseNumberAddition = txtCustomerAddressHouseNumberAddition.Text.NullIfEmpty(),
                                Street = txtCustomerAddressStreet.Text.NullIfEmpty(),
                                City = txtCustomerAddressCity.Text.NullIfEmpty(),
                                Region = txtCustomerAddressRegion.Text.NullIfEmpty(),
                                Country = ddlCustomerAddressCountry.SelectedValue.NullIfEmpty()
                            }
                    }
            };
            request.Customer.Company = new PostcodeNlSignalCompany
            {
                Name = txtCustomerCompanyName.Text.NullIfEmpty(),
                GovernmentId = txtCustomerCompanyGovernmentId.Text.NullIfEmpty(),
                Country = ddlCustomerCompanyCountry.SelectedValue.NullIfEmpty()
            };
            request.Access = new PostcodeNlSignalAccess
            {
                IpAddress = txtAccessIpAddress.Text.NullIfEmpty(),
                AdditionalIpAddresses =
                    txtAccessAdditionalUpAddresses.Text.Split(new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries).ToList(),
                SessionId = txtAccessSessionId.Text.NullIfEmpty(),
                Time = DateTime.TryParse(txtAccessTime.Text, out time) ? (DateTime?)time : null,
                Browser =
                    new PostcodeNlSignalBrowser
                    {
                        UserAgent = txtAccessBrowserUserAgent.Text.NullIfEmpty(),
                        AcceptLanguage = txtAccessBrowserAcceptLanguage.Text.NullIfEmpty()
                    }
            };
            request.Transaction = new PostcodeNlSignalTransaction
            {
                InternalId = txtTransactionInternalId.Text.NullIfEmpty(),
                Status = ddlTransactionStatus.SelectedValue.NullIfEmpty(),
                Cost = double.TryParse(txtTransactionCost.Text, out cost) ? (double?)cost : null,
                CostCurrency = txtTransactionCostCurrency.Text.NullIfEmpty(),
                PaymentType = txtTransactionPaymentType.Text.NullIfEmpty(),
                Weight = int.TryParse(txtTransactionWeight.Text, out weight) ? (int?)weight : null,
                DeliveryAddress =
                    new PostcodeNlSignalAddress
                    {
                        Postcode = txtTransactionDeliveryAddressPostcode.Text.NullIfEmpty(),
                        HouseNumber =
                            int.TryParse(txtTransactionDeliveryAddressHouseNumber.Text, out houseNumber)
                                ? (int?)houseNumber
                                : null,
                        HouseNumberAddition = txtTransactionDeliveryAddressHouseNumberAddition.Text.NullIfEmpty(),
                        Street = txtTransactionDeliveryAddressStreet.Text.NullIfEmpty(),
                        City = txtTransactionDeliveryAddressCity.Text.NullIfEmpty(),
                        Region = txtTransactionDeliveryAddressRegion.Text.NullIfEmpty(),
                        Country = ddlTransactionDeliveryAddressCountry.SelectedValue.NullIfEmpty()
                    }
            };
            request.Config = new PostcodeNlSignalConfig
            {
                SelectServices =
                    txtConfigSelectServices.Text.Split(new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries).ToList(),
                ExcludeServices =
                    txtConfigExcludeServices.Text.Split(new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries).ToList(),
                SelectTypes =
                    txtConfigSelectTypes.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                ExcludeTypes =
                    txtConfigExcludeTypes.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList()
            };

            var result = client.DoSignalCheck(request);

            lSignalCount.Text = string.Format("{0}", result.Signals.Count);
            hplReportPdfUrl.NavigateUrl = result.ReportPdfUrl;

            lResult.Text = result.ToString();
            pnlResult.Visible = true;
            pnlSignalResponse.Visible = true;
        }
        catch (PostcodeNlApiRestClientClientException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Client error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientServiceException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Service error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientInputInvalidException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Input error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientAuthenticationException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Authentication error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (PostcodeNlApiRestClientAddressNotFoundException ex)
        {
            pnlError.Visible = true;
            lType.Text = "Address not found";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }
        catch (Exception ex)
        {
            pnlError.Visible = true;
            lType.Text = "Error";
            lMessage.Text = ex.Message;
            lClass.Text = ex.GetType().ToString();
        }

        if (chkSignalShowRawRequestResponse.Checked)
        {
            pnlRawRequestResponse.Visible = true;
            var debugData = client.GetDebugData();
            if (debugData != null)
            {
                lRequest.Text = debugData["request"];
                lResponse.Text = debugData["response"];
            }
        }
        pnlTimeTaken.Visible = true;
        lTimeTaken.Text = DateTime.Now.Subtract(start).TotalSeconds.ToString();
    }
}