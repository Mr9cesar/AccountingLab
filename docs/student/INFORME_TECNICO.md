# Informe técnico - AccountingLab

## Entorno de ejecución

- Sistema operativo: Windows.
- Backend: C# y .NET 8.
- Frontend: React, TypeScript y Vite.
- Base de datos: MySQL 8.0.46.
- Node.js: 26.13.1.

## Resultados de pruebas

| Categoría | Total | Aprobadas | Fallidas |
|---|---:|---:|---:|
| Unitarias | 93 | 93 | 0 |
| Integración | 76 | 76 | 0 |
| Arquitectura | 6 | 6 | 0 |
| Patrones de diseño | 66 | 66 | 0 |
| Frontend | 25 | 25 | 0 |
| Total | 266 | 266 | 0 |

La comprobación de integridad también fue aprobada.

## Comando de verificación

pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/Invoke-Tests.ps1 -Category All -ResultsDirectory reports/student

## Evidencias

[Reporte de ejecución](../../reports/student/SUMMARY.md)

## Verificación de la interfaz

Se ejecutaron las demostraciones de los 22 desafíos
desde la interfaz web sin errores visibles.

## Limitaciones

- Docker no fue verificado.
- Se utilizó MySQL local.
- Las pruebas aprobadas no certifican por sí solas
  la correcta aplicación estructural de cada patrón.

## Transparencia sobre el uso de IA

Se utilizó ChatGPT para orientación técnica, generación
de código de implementación, resolución de errores
y configuración del entorno. Parte de esta asistencia
excedió los límites de la política académica del proyecto.
