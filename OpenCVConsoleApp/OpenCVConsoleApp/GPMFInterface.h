#pragma once


struct GPMF_stream;
class GPMFInterface
{
public:
	

	int Run(char *filename, char * output_filename);
	void printfData(uint32_t type, uint32_t structsize, uint32_t repeat, void *data);
	void PrintGPMF(GPMF_stream *ms);
	std::string m_func_name;
	std::string m_start_time;
	int m_dest_resolution;
protected:
	const static cv::Size imageSize;
	const static cv::Matx33d K;
	const static cv::Vec<double, 5> D;
	std::string datasets_repository_path;
	
private:
	
	void Cleanup();
	uint32_t * m_payload;  //buffer to store GPMF samples from the MP4.
	uint32_t m_payloads;
	float m_metadatalength;

	void StartWriteFile(const char * filename);
	void EndWriteFile();

	float SHUT_rate;
	float ACCL_rate;
	float GYRO_rate;
	float GPS5_rate;
};