using System;
using System.Web.Configuration;
using PostcodeNlApi;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlResult.Visible = false;
        pnlError.Visible = false;
        pnlTimeTaken.Visible = false;
        pnlRawRequestResponse.Visible = false;
        if (!IsPostBack)
        {
            //txtApiUrl.Text = "https://api.postcode.nl/rest";
            txtApiUrl.Text = "https://api.postcode.eu/nl/v1";
            txtKey.Text = WebConfigurationManager.AppSettings["appKey"];
            txtSecret.Text = WebConfigurationManager.AppSettings["appSecret"];
        }
    }

    protected void btnAddressSend_Click(object sender, EventArgs e)
    {
        DateTime start = DateTime.Now;

        var client = new PostcodeNlApiRestClient(txtKey.Text, txtSecret.Text, txtApiUrl.Text);
        client.SetDebugEnabled();
        try
        {
            var result = client.LookupAddress(txtPostcode.Text, txtHouseNumber.Text, txtHouseNumberAddition.Text,
                chkValidateHouseNumberAddition.Checked);

            lStreet.Text = result.Street;
            lHouseNumber.Text = result.HouseNumber;
            lHouseNumberAddition.Text = !string.IsNullOrEmpty(result.HouseNumberAddition) ? result.HouseNumberAddition : txtHouseNumberAddition.Text;
            lPostcode.Text = result.Postcode;
            lCity.Text = result.City;
            lResult.Text = result.ToString();
            pnlResult.Visible = true;
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

        pnlRawRequestResponse.Visible = true;
        var debugData = client.GetDebugData();
        if (debugData != null)
        {
            lRequest.Text = debugData["request"];
            lResponse.Text = debugData["response"];
        }
        pnlTimeTaken.Visible = true;
        lTimeTaken.Text = DateTime.Now.Subtract(start).TotalSeconds.ToString();
    }
}