using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

using FluidEarth2.Sdk;
using FluidEarth2.Sdk.CoreStandard2;
using FluidEarth2.Sdk.Interfaces;
using OpenMI.Standard2;

using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace bmiFlaskOpenmi2
{
    public class bmiFlaskOpenMI2Wrapper : FluidEarth2.Sdk.Interfaces.IEngineTime
    {

        string id = null;
        string model = null;

        public double GetCurrentTime()
        {
            throw new NotImplementedException();
        }

        public void Finish()
        {
            //call the BMI finalize method
            var client = new RestClient("http://localhost:5000");
            var request = new RestRequest("/models/{model}/{id}/finalize", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment);
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            IRestResponse response = client.Execute(request);

            //get modelID from response
            dynamic r = JObject.Parse(response.Content);
            //todo parse response
        }

        public bool[] GetBooleans(string engineVariable, bool missingValue)
        {
            throw new NotImplementedException();
        }

        public double[] GetDoubles(string engineVariable, double missingValue)
        {
            //--call get_values BMI method
            var client = new RestClient("http://localhost:5000");
            var request = new RestRequest("/models/{model}/{id}/{long_var_name}/get_values", Method.GET);
            request.AddParameter("model", this.model, ParameterType.UrlSegment);
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            request.AddParameter("long_var_name", engineVariable, ParameterType.UrlSegment);
            IRestResponse response = client.Execute(request);

            //get result
            dynamic r = JObject.Parse(response.Content);
            //todo parse response to get result
            double[] result = {1,2};

            return result;
        }

        public int[] GetInt32s(string engineVariable, int missingValue)
        {
            throw new NotImplementedException();
        }

        public string[] GetStrings(string engineVariable, string missingValue)
        {
            throw new NotImplementedException();
        }

        //initialisingText is component name
        public void Initialise(string initialisingText)
        {   
            //--hard code model name here only. This can come from initialisingText in future.
            this.model = "met_base";

            //--instantiate the component
            var client = new RestClient("http://localhost:5000");
            var request = new RestRequest("models/{model}/instantiate", Method.GET);
            request.AddParameter("model", this.model, ParameterType.UrlSegment);
            IRestResponse response = client.Execute(request);

            //get modelID from response
            dynamic d = JObject.Parse(response.Content);
            string id = d.ID.Value;
            this.id = id;

            //--set the prefixes for the component
            request = new RestRequest("models/{model}/{id}/set_case_site_prefix", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            //payload = {'case_prefix': case_prefix, 'site_prefix': site_prefix}
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(
                new
                {
                    case_prefix = "June_12_1", 
                    site_prefix = "Owl_30m"
                }); //TODO remvoe hard coding
            response = client.Execute(request);
            //TODO parse response

            //--set the vars provided
            request = new RestRequest("models/{model}/{id}/set_vars_provided_list", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(
                new
                {
                    //vars_provided = "snowpack__z_mean_of_mass-per-volume_density",
                     vars_provided =  "snowpack__depth" //,
                    //"snowpack__liquid-equivalent_depth",
                    // "snowpack__melt_volume_flux"
                }); //TODO remvoe hard coding
            response = client.Execute(request);
            //TODO parse response 

            //--send the cfg and sup files to the component (todo need to clean this up)     
            request = new RestRequest("models/{model}/{id}/send_cfg_sup_files", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            FileStream fileStream = null;
            string filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\June_12_1_meteorology.cfg";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("cfg_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\TFDEM_DEM.tif";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\precipitation-mm_data.txt";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\relative-humidity_data.txt";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\temperature-c_data.txt";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\Owl_30m_slope.tif";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);
            filePath = @"C:\Users\jlg\Documents\openmi2\bmi-flask-openmi2-wrapper\data\Owl_test_30m\Owl_30m_aspect.tif";
            using (fileStream = File.OpenRead(filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            request.AddFile("sup_file", filePath);

            //Execute client and parse response
            response = client.Execute(request);
            //TODO parse response

            //initialize the component
            request = new RestRequest("models/{model}/{id}/initialize", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            response = client.Execute(request);
            // TODO parse response

        }

        public string Ping()
        {
            var client = new RestClient("http://localhost:5000");
            string id = this.id; 

            //--set the prefixes for the component
            var request = new RestRequest("/models/{model}/{id}/status", Method.POST);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);

            //--parse response
            IRestResponse response = client.Execute(request);
            dynamic d = JObject.Parse(response.Content);
            string status = d.status.Value;
            return status;
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public void SetArgument(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetBooleans(string engineVariable, bool missingValue, bool[] values)
        {
            throw new NotImplementedException();
        }

        public void SetDoubles(string engineVariable, double missingValue, double[] values)
        {
            throw new NotImplementedException();
        }

        public void SetInput(string engineVariable, int elementCount, int[] elementValueCounts, int vectorLength)
        {
            throw new NotImplementedException();
        }

        public void SetInput(string engineVariable, int elementCount, int elementValueCount, int vectorLength)
        {
            throw new NotImplementedException();
        }

        public void SetInt32s(string engineVariable, int missingValue, int[] values)
        {
            throw new NotImplementedException();
        }

        public void SetOutput(string engineVariable, int elementCount, int[] elementValueCounts, int vectorLength)
        {
            throw new NotImplementedException();
        }

        public void SetOutput(string engineVariable, int elementCount, int elementValueCount, int vectorLength)
        {
            throw new NotImplementedException();
        }

        public void SetStrings(string engineVariable, string missingValue, string[] values)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            //--instantiate the component
            var client = new RestClient("http://localhost:5000");
            string id = this.id; //TODO remove hardcoding

            //--set the prefixes for the component
            var request = new RestRequest("/models/{model}/{id}/update", Method.PUT);
            request.AddParameter("model", this.model, ParameterType.UrlSegment); //TODO remove hardcoded 
            request.AddParameter("id", this.id, ParameterType.UrlSegment);
            request.AddQueryParameter("download_updatenc", "False"); //TODO remove hardcoded 

            IRestResponse response = client.Execute(request);
            //TODO parse response
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}
