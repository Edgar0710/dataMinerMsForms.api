using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using dataMiner.Data;
using DataMiner.Model;
using DataMinerBussiness.IBussiness;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace dataMinerMsForms.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   public class FormController : ControllerBase
    {
        IformBusiness formb;
        public FormController(IformBusiness _iformBusiness)
        {
            formb = _iformBusiness;
        }
        #region Get
        [HttpGet, Route("GetForms")]
        public Response<object> GetForms(int usuario) { 
        
            return formb.GetForms(usuario);
        }
        [HttpGet, Route("GetRespuestaFormulario")]
        public Response<object> GetRespuestaFormulario(int form)
        {

            return formb.GetRespuestaFormulario(form);
        }

        #endregion
        #region Post
        [HttpPost, Route("InsertaForm")]
        public async Task<Response<object>> InsertaFormAsync(string nombre,string descripcion, int usuario,IFormFile file)
        {
            Response<object> response = new Response<object>();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        int formid = formb.InsertaForm(nombre, descripcion, usuario).Result;
                        // Ejemplos de acceso a datos
                        DataTable table = result.Tables[0];
                        for (int i = 6; i < table.Columns.Count; i++)
                        {
                            for(int j=1;j< table.Rows.Count; j++)
                            {
                                var a = formb.InsertaRespuestas(table.Rows[j][5].ToString(), table.Rows[j][3].ToString(), table.Rows[0][i].ToString(), table.Rows[j][i].ToString(), formid);
                            }
                        }

                        response.Result = formid;
                        response.Code = ResponseEnum.Ok;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Menssage = "Error al procesar el archivo";
                response.Result = null;
                response.Code = ResponseEnum.Ok;
            }
            //return formb.InsertaForm(nombre,descripcion,usuario);

            return response;
        }

        [HttpPost, Route("InsertaRespuestas")]
        public Response<object> InsertaRespuestas(string en_nombre, string en_email, string pregunta,
         string respuesta ,  int formulario)
        {



            return formb.InsertaRespuestas(en_nombre, en_email, pregunta,respuesta,formulario);
        }


        #endregion

    }
}