using DevelWebApi.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevelWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EncuestaController : ControllerBase
    {
        [HttpGet("GetEncuesta", Name = "GetEncuesta")]
        public dynamic GetEncuesta(IConfiguration configuration, [FromQuery] List<long> pListadoEncuestaId)
        {
            string resultado = string.Empty;

            try
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;

                List<Encuesta> listadoEncuesta = OperacionEncuesta.GetEncuesta(configuration, pListadoEncuestaId);
                resultado = JsonConvert.SerializeObject(listadoEncuesta, Formatting.Indented, jsonSerializerSettings);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al consultar Encuestas: {ex.Message}");
            }
        }

        [HttpGet("GetTipoCampo", Name = "GetTipoCampo")]
        public dynamic GetTipoCampo(IConfiguration configuration)
        {
            string resultado = string.Empty;

            try
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;

                List<TipoCampo> listadoTipoCampo = OperacionEncuesta.GetTipoCampo(configuration);
                resultado = JsonConvert.SerializeObject(listadoTipoCampo, Formatting.Indented, jsonSerializerSettings);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al consultar tipo  de campos: {ex.Message}");
            }
        }

        [HttpPost("PostEncuesta", Name = "PostEncuesta")]
        public dynamic PostEncuesta(IConfiguration configuration, [FromQuery] string encuestaDescripcion, [FromBody] List<pCampos> campos)
        {
            string resultado = string.Empty;

            try
            {
                OperacionEncuesta.PostEncuesta(configuration, encuestaDescripcion, campos);
                return Ok("exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al insertar encuesta: {ex.Message}");
            }
        }

        [HttpPost("PostCampoEncuesta", Name = "PostCampoEncuesta")]
        public dynamic PostCampoEncuesta(IConfiguration configuration, [FromQuery] long encuestaId, [FromBody] List<pCampos> campos)
        {
            string resultado = string.Empty;

            try
            {
                if (campos.Count > 0)
                {
                    foreach (var item in campos)
                    {
                        OperacionEncuesta.PostCampos(configuration, encuestaId, item);
                    }
                }

                return Ok("exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al insertar campo: {ex.Message}");
            }
        }

        [HttpPut("PutEncuesta", Name = "PutEncuesta")]
        public dynamic PutEncuesta(IConfiguration configuration, [FromQuery] long encuestaId, [FromQuery] string encuestaDescripcion)
        {
            string resultado = string.Empty;

            try
            {
                OperacionEncuesta.PutEncuesta(configuration, encuestaId, encuestaDescripcion);
                return Ok("exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al modificar encuesta: {ex.Message}");
            }
        }

        [HttpPut("PutCampo", Name = "PutCampo")]
        public dynamic PutCampo(IConfiguration configuration, [FromQuery] long campoId, [FromBody] pCampos pCampo)
        {
            string resultado = string.Empty;

            try
            {
                OperacionEncuesta.PutCampo(configuration, campoId, pCampo);
                return Ok("exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al modificar campo: {ex.Message}");
            }
        }

        [HttpDelete("DeleteEncuesta", Name = "DeleteEncuesta")]
        public dynamic DeleteEncuesta(IConfiguration configuration, [FromQuery] long encuestaId)
        {
            string resultado = string.Empty;

            try
            {
                OperacionEncuesta.DeleteEncuesta(configuration, encuestaId);
                return Ok("exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al modificar campo: {ex.Message}");
            }
        }
    }
}
