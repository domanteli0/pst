default: run-and-cleanup

run:
    JVM_ARGS="-Xms1024m -Xmx1024m" jmeter -n -t lab3.jmx \
         -l jmeter/results.csv -j jmeter/jmeter.log -e -o jmeter/results

open:
    JVM_ARGS="-Xms1024m -Xmx1024m" jmeter -t ND1Complete.jmx \
         -l jmeter/results.csv -j jmeter/jmeter.log -e -o jmeter/results

cleanup:
    mkdir jmeter/plain
    mv *.plain jmeter/plain/

run-and-cleanup: run cleanup

clear:
    trash jmeter *.plain jmeter.log *.unknown
