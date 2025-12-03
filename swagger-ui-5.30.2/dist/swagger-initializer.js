window.onload = function() {
  //<editor-fold desc="Changeable Configuration Block">

  // the following lines will be replaced by docker/configurator, when it runs in a docker-container
  window.ui = SwaggerUIBundle({
    urls: [
      {
        url: "/api/swagger/auth/swagger.json",
        name: "Auth API"
      },
      {
        url: "/api/swagger/fg_inventory_mobile/swagger.json",
        name: "FG Inventory API"
      },
      {
        url: "/api/swagger/systemmaster/swagger.json",
        name: "System API"
      }
    ],
    dom_id: '#swagger-ui',
    deepLinking: true,
    presets: [
      SwaggerUIBundle.presets.apis,
      SwaggerUIStandalonePreset
    ],
    plugins: [
      SwaggerUIBundle.plugins.DownloadUrl
    ],
    layout: "StandaloneLayout"
  });

  //</editor-fold>
};
