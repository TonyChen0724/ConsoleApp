#include "stdafx.h"
#include <stdio.h>
#include <windows.h>
#include "GPMFInterface.h"
#include "GPMF_mp4reader.h"
#include "GPMF_parser.h"

#include <iostream>
#include <fstream>
#include <time.h>

#define VERBOSE_OUTPUT		0

#if VERBOSE_OUTPUT
#define LIMITOUTPUT		arraysize = structsize;
#else
#define LIMITOUTPUT		if (arraysize > 1 && repeat > 3) repeat = 3, dots = 1; else if (repeat > 6) repeat = 6, dots = 1;
#endif


std::ofstream g_fstream;
bool is_start_write_file;
void FormatWriteFile(const char * fmt, ...)
{
	char buffer[255];
	va_list vl;
	va_start(vl, fmt);
	//printf(fmt, vl);
	if (is_start_write_file)
	{
		int nsize = vsnprintf(buffer, 255, fmt, vl);
		g_fstream << buffer << std::endl;
	}
//	std::string ret(buffer);
	va_end(vl);
	return;
}

void GPMFInterface::PrintGPMF(GPMF_stream *ms)
{
	if (ms)
	{
		uint32_t key = GPMF_Key(ms);
		uint32_t type = GPMF_Type(ms);
		uint32_t structsize = GPMF_StructSize(ms);
		uint32_t repeat = GPMF_Repeat(ms);
		uint32_t size = GPMF_RawDataSize(ms);
		uint32_t level = GPMF_NestLevel(ms);
		void *data = GPMF_RawData(ms);

		if (key != GPMF_KEY_DEVICE) level++;

		while (level > 0 && level < 10)
		{
			printf("  ");
			level--;
		}
		if (type == 0)
			printf("%c%c%c%c nest size %d ", (key >> 0) & 0xff, (key >> 8) & 0xff, (key >> 16) & 0xff, (key >> 24) & 0xff, size);
		else if (structsize == 1 || (repeat == 1 && type != '?'))
			printf("%c%c%c%c type '%c' size %d ", (key >> 0) & 0xff, (key >> 8) & 0xff, (key >> 16) & 0xff, (key >> 24) & 0xff, type == 0 ? '0' : type, size);
		else
			printf("%c%c%c%c type '%c' struct %d repeat %d ", (key >> 0) & 0xff, (key >> 8) & 0xff, (key >> 16) & 0xff, (key >> 24) & 0xff, type == 0 ? '0' : type, structsize, repeat);

		if (type && repeat > 0)
		{
			printf("data: ");

			if (type == GPMF_TYPE_COMPLEX)
			{
				GPMF_stream find_stream;
				GPMF_CopyState(ms, &find_stream);
				if (GPMF_OK == GPMF_FindPrev(&find_stream, GPMF_KEY_TYPE, GPMF_CURRENT_LEVEL))
				{
					char *srctype = (char *)GPMF_RawData(&find_stream);
					uint32_t typelen = GPMF_RawDataSize(&find_stream);
					int struct_size_of_type;

					struct_size_of_type = GPMF_SizeOfComplexTYPE(srctype, typelen);
					if (struct_size_of_type != (int32_t)structsize)
					{
						printf("error: found structure of %d bytes reported as %d bytes", struct_size_of_type, structsize);
					}
					else
					{
						char typearray[64];
						uint32_t elements = sizeof(typearray);
						uint8_t *bdata = (uint8_t *)data;
						uint32_t i;

						if (GPMF_OK == GPMF_ExpandComplexTYPE(srctype, typelen, typearray, &elements))
						{
#if VERBOSE_OUTPUT
							uint32_t j;
							for (j = 0; j < repeat; j++)
							{
								if (repeat > 1) DBG_MSG("\n");
#endif
								for (i = 0; i < elements; i++)
								{
									int elementsize = GPMF_SizeofType((GPMF_SampleType)typearray[i]);
									printfData(typearray[i], elementsize, 1, bdata);
									bdata += elementsize;
								}
#if VERBOSE_OUTPUT
							}
#else
								if (repeat > 1)
									printf("...");
#endif
						}
					}
				}
				else
				{
					printf("unknown formatting");
				}
			}
				else
				{
					printfData(type, structsize, repeat, data);
				}
		}

			printf("\n");
	}
}



void GPMFInterface::StartWriteFile(const char * filename)
{
	is_start_write_file = true;
	g_fstream.open(filename);
	//std::string data;
	///data = "Function: ";
	//data.append(m_func_name.c_str());
	//m_fstream << data.c_str() << std::endl;
}
void GPMFInterface::EndWriteFile()
{
	g_fstream.close();
	is_start_write_file = false;
}
int GPMFInterface::Run(char * filename, char * output_filename)
{
	int32_t ret = GPMF_OK;
	GPMF_stream metadata_stream, *ms = &metadata_stream;
	m_payload = NULL; 
	m_metadatalength = OpenGPMFSource(filename);
	if (m_metadatalength <= 0.0)
	{
		Cleanup();
		return ret;
	}

	StartWriteFile(output_filename);

	uint32_t index, m_payloads = GetNumberGPMFPayloads();
	FormatWriteFile("MetadataLength=%.2f,Payload=%d,Filename=%s,", m_metadatalength, m_payloads, filename);
	if (m_payloads == 1) // Printf the contents of the single payload
	{
		uint32_t payloadsize = GetGPMFPayloadSize(0);
		m_payload = GetGPMFPayload(m_payload, 0);
		if (m_payload == NULL)
		{
			Cleanup();
			return ret;
		}
		ret = GPMF_Init(ms, m_payload, payloadsize);
		if (ret != GPMF_OK)
		{
			Cleanup();
			return ret;
		}
		// Output (printf) all the contained GPMF data within this payload
		ret = GPMF_Validate(ms, GPMF_RECURSE_LEVELS); // optional
		if (GPMF_OK != ret)
		{
			printf("Invalid Structure\n");
			Cleanup();
			return ret;
		}

		GPMF_ResetState(ms);
		do
		{
			PrintGPMF(ms);  // printf current GPMF KLV
		} 
		while (GPMF_OK == GPMF_Next(ms, GPMF_RECURSE_LEVELS));
		GPMF_ResetState(ms);
		printf("\n");
	}



	for (index = 0; index < m_payloads; index++)
	{
		uint32_t payloadsize = GetGPMFPayloadSize(index);
		float in = 0.0, out = 0.0; //times
		m_payload = GetGPMFPayload(m_payload, index);
		if (m_payload == NULL)
		{
			Cleanup();
			return ret;
		}

		ret = GetGPMFPayloadTime(index, &in, &out);
		if (ret != GPMF_OK)
		{
			Cleanup();
			return ret;
		}
		FormatWriteFile("PayloadInTime=%.3f,PayloadOutTime=%.3f,", in, out);

		ret = GPMF_Init(ms, m_payload, payloadsize);
		if (ret != GPMF_OK)
		{
			Cleanup();
			return ret;
		}

		// Find all the available Streams and the data carrying FourCC
		while (GPMF_OK == GPMF_FindNext(ms, GPMF_KEY_STREAM, GPMF_RECURSE_LEVELS))
		{
			if (GPMF_OK == GPMF_SeekToSamples(ms)) //find the last FOURCC within the stream
			{
				uint32_t key = GPMF_Key(ms);
				GPMF_SampleType type = (GPMF_SampleType)GPMF_Type(ms);
				uint32_t elements = GPMF_ElementsInStruct(ms);
				uint32_t samples = GPMF_Repeat(ms);
				if (samples)
				{
					float rate = GetGPMFSampleRateAndTimes(ms, 0.0, index, &in, &out);
					FormatWriteFile("%c%c%c%c In=%.3f,Out=%.3f,rate=%.3f", PRINTF_4CC(key), in, out, rate);

					if (type == GPMF_TYPE_COMPLEX)
					{
						GPMF_stream find_stream;
						GPMF_CopyState(ms, &find_stream);
						if (GPMF_OK == GPMF_FindPrev(&find_stream, GPMF_KEY_TYPE, GPMF_CURRENT_LEVEL))
						{
							char tmp[64];
							char *data = (char *)GPMF_RawData(&find_stream);
							int size = GPMF_RawDataSize(&find_stream);
							if (size < sizeof(tmp))
							{
								memcpy(tmp, data, size);
								tmp[size] = 0;
								printf("of type %s ", tmp);
							}
						}
					}
					else
					{
						printf("of type %c ", type);
					}

					FormatWriteFile("%c%c%c%c Samples=%d,", PRINTF_4CC(key),samples);
					if (elements > 1)
						FormatWriteFile("%c%c%c%c Elements=%d,", PRINTF_4CC(key), elements);
					printf("\n");
				}
			}
		}
		GPMF_ResetState(ms);
		printf("\n");

		// Find GPS values and return scaled floats. 
		if (GPMF_OK == GPMF_FindNext(ms, STR2FOURCC("GPS5"), GPMF_RECURSE_LEVELS) || //GoPro Hero5 GPS
			GPMF_OK == GPMF_FindNext(ms, STR2FOURCC("GPRI"), GPMF_RECURSE_LEVELS))   //GoPro Karma GPS
		{
			uint32_t samples = GPMF_Repeat(ms);
			uint32_t elements = GPMF_ElementsInStruct(ms);
			uint32_t buffersize = samples * elements * sizeof(float);
			GPMF_stream find_stream;
			float *ptr, *tmpbuffer = (float *)malloc(buffersize);
			char units[10][6] = { "" };
			uint32_t unit_samples = 1;

			if (tmpbuffer && samples)
			{
				uint32_t i, j;

				//Search for any units to display
				GPMF_CopyState(ms, &find_stream);
				if (GPMF_OK == GPMF_FindPrev(&find_stream, GPMF_KEY_SI_UNITS, GPMF_CURRENT_LEVEL) ||
					GPMF_OK == GPMF_FindPrev(&find_stream, GPMF_KEY_UNITS, GPMF_CURRENT_LEVEL))
				{
					char *data = (char *)GPMF_RawData(&find_stream);
					int ssize = GPMF_StructSize(&find_stream);
					unit_samples = GPMF_Repeat(&find_stream);

					for (i = 0; i < unit_samples; i++)
					{
						memcpy(units[i], data, ssize);
						units[i][ssize] = 0;
						data += ssize;
					}
				}

				//GPMF_FormattedData(ms, tmpbuffer, buffersize, 0, samples); // Output data in LittleEnd, but no scale
				GPMF_ScaledData(ms, tmpbuffer, buffersize, 0, samples, GPMF_TYPE_FLOAT);  //Output scaled data as floats
				ptr = tmpbuffer;
				for (i = 0; i < samples; i++)
				{
					for (j = 0; j < elements; j++)
						FormatWriteFile("GPSElement%d=%.7f,", j + 1, *ptr++);

						//FormatWriteFile("GPSElement%d=%.7f%s,",j+1, *ptr++, units[j%unit_samples]);

					printf("\n");
				}
				free(tmpbuffer);
			}
		}
		GPMF_ResetState(ms);
		printf("\n");

		// determine the samples for particular streams
		uint32_t fourcc = STR2FOURCC("SHUT");
		float rate = GetGPMFSampleRate(fourcc);
		FormatWriteFile("%c%c%c%c Rate=%.3f", PRINTF_4CC(fourcc), rate);

		fourcc = STR2FOURCC("ACCL");
		rate = GetGPMFSampleRate(fourcc);
		FormatWriteFile("%c%c%c%c Rate=%.3f", PRINTF_4CC(fourcc), rate);

		fourcc = STR2FOURCC("GYRO");
		rate = GetGPMFSampleRate(fourcc);
		FormatWriteFile("%c%c%c%c Rate=%.3f", PRINTF_4CC(fourcc), rate);

		fourcc = STR2FOURCC("GPS5");
		rate = GetGPMFSampleRate(fourcc);
		FormatWriteFile("%c%c%c%c Rate=%.3f", PRINTF_4CC(fourcc), rate);

	}

	Cleanup();
	EndWriteFile();
	return ret;
}

void GPMFInterface::Cleanup()
{
	if (m_payload)
		FreeGPMFPayload(m_payload); 
	m_payload = NULL;
	CloseGPMFSource();
}

void GPMFInterface::printfData(uint32_t type, uint32_t structsize, uint32_t repeat, void *data)
{

}