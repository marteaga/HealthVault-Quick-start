/********************************************************************++

Copyright (c) Microsoft Corporation. All rights reserved.

************************************************************************/

using System;
using System.Data;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Microsoft.Health;
using Microsoft.Health.Web;
using Microsoft.Health.ItemTypes;

public partial class _Default : HealthServicePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            ApplicationInfo info = ApplicationConnection.GetApplicationInfo();
            AppName.Text += info.Name;
            AppId.Text += info.Id.ToString();

            // set the persons name 
            name.Text += PersonInfo.Name;

            // attempt to get the birthdate of the user
            Basic basic = GetSingleValue<Basic>(Basic.TypeId);
            if (basic != null && basic.BirthYear.HasValue)
                bday.Text += (basic.BirthYear.ToString());
            StartupData.SetActiveView(StartupData.Views[0]);
        }
        catch (HealthServiceException ex)
        {
            Error.Text += ex.ToString();
            StartupData.SetActiveView(StartupData.Views[1]);
        }
    }

    /// <summary>
    /// Generic method to get data from healthvault depending on the TypeId
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeID"></param>
    /// <returns></returns>
    T GetSingleValue<T>(Guid typeID) where T : class
    {
        // create a searcher to get data from healthvault
        HealthRecordSearcher searcher = PersonInfo.SelectedRecord.CreateSearcher();

        // create a filter to add to the search
        HealthRecordFilter filter = new HealthRecordFilter(typeID);
        searcher.Filters.Add(filter);

        // make the request to find the data
        HealthRecordItemCollection items = searcher.GetMatchingItems()[0];

        // return the data if available
        if (items != null && items.Count > 0)
        {
            return items[0] as T;
        }
        else
        {
            return null;
        }
    }
}
