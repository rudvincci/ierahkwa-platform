use std::path::{Path, PathBuf};

fn collect_proto_files(dir: &Path, out: &mut Vec<PathBuf>) -> std::io::Result<()> {
    for entry in std::fs::read_dir(dir)? {
        let entry = entry?;
        let path = entry.path();
        if path.is_dir() {
            collect_proto_files(&path, out)?;
            continue;
        }
        if path.extension().and_then(|e| e.to_str()) == Some("proto") {
            out.push(path);
        }
    }
    Ok(())
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Proto files are in the MameyNode repository.
    // From this crate (`MameyNode.Rust`), proto dir is:
    // `../MameyNode/proto`
    let proto_dir = PathBuf::from("../MameyNode/proto");

    println!("cargo:rerun-if-changed=build.rs");

    let mut proto_files: Vec<PathBuf> = Vec::new();
    collect_proto_files(&proto_dir, &mut proto_files)?;
    proto_files.sort_by(|a, b| a.to_string_lossy().cmp(&b.to_string_lossy()));

    for proto_path in &proto_files {
        println!("cargo:rerun-if-changed={}", proto_path.display());
    }

    let proto_paths: Vec<&Path> = proto_files.iter().map(|p| p.as_path()).collect();
    tonic_build::configure()
        .build_server(false)
        .build_client(true)
        .compile(&proto_paths, &[&proto_dir])?;

    Ok(())
}


