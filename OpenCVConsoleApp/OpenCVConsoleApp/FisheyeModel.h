#pragma once


struct camera_config
{
	float focallen;
	float r1;
	float r2;
	float r3;
	float ppx;
	float ppy;
};
class fisheyeTest
{

public:
	bool LoadConfig(char * filename);
	void MergeVirb360Image(const char * filename, const char * output_filename);
	void ChangeResolution(const char * filename, const char * output_filename, int resolutionX, int resolutionY);
	void ChangeResolution(const char * filename, const char * output_filename,int resolution);
	void undistortImage(int idx,const char * filename, const char * output_filename);
	void undistortImageWithCamConfig(int idx, const char * filename, const char * output_filename);
	void CopyImage(char * filename, char * output_filename);

	void fish2sphere(cv::Mat &inFisheye, cv::Mat &MapX, cv::Mat &MapY, float fishFOV);

	void InitLog();
	void BuildLogFile(const char * output_folder,int type);
	void copiedInitialize(cv::Mat &in);
	void createCubeMapMappingFromFisheye200(cv::Mat &in, cv::Mat &MapX, cv::Mat &MapY, int faceId, const int width, const int height);
	void createCubeMapFace(cv::Mat &in, cv::Mat &MapX, cv::Mat &MapY, int faceId, const int width, const int height);
	//void undistortImage_Fisheye(char * filename, char * output_filename);
	int m_is_dir;
	char m_process_directory1[255];
	char m_process_directory2[255];
	char m_process_directory3[255];
	char m_process_directory4[255];

	camera_config m_cam_config;
	float m_dest_focal_length;
	int m_count_process;
	std::string m_func_name;
	std::string m_start_time;
	int m_dest_resolution;
protected:
	const static cv::Size imageSize;
	const static cv::Matx33d K;
	const static cv::Vec<double, 5> D;
	std::string datasets_repository_path;
	
	cv::Mat mergeRectification(const cv::Mat& l, const cv::Mat& r);	
	float GetFocalLengthInPixel(float focallen, float width,float crop);
	float GetFocalLengthInPixelFrom36mmFocalLength(float focallen, float width);


	//void createCubeMapFace(const cv::Mat &in, cv::Mat &face, int faceId = 0, const int width = -1, const int height = -1); moved to public for now
private:
	float m_src_focallen[4];
	float m_r1[4];
	float m_r2[4];
	float m_r3[4];
	float m_ppx[4];
	float m_ppy[4];
	float m_dest_focallen[4];
	int m_limited_range;
	int m_is_test_limited_range;
	int m_cutoff_top_buttom;
	float fov;
};