-- Limpiar todos los datos para presentación
-- Mantener estructura y configuraciones

-- Limpiar registros de datos
DELETE FROM asistencias;
DELETE FROM clase_inscripciones;
DELETE FROM pagos;
DELETE FROM socios;
DELETE FROM clases;

-- Resetear contadores eliminando las secuencias (para empezar desde 1)
DELETE FROM sqlite_sequence WHERE name IN ('socios', 'pagos', 'asistencias', 'clases', 'clase_inscripciones');

-- Mantener planes y usuarios intactos
-- Mantener configuración de email intacta